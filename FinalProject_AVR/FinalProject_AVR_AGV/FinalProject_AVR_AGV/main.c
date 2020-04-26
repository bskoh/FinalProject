/*
* Final_Project_AGV.c
*
* Created: 2020-03-18 오후 7:42:56
* Author : kccistc
*/

#define F_CPU 16000000UL

#include <avr/io.h>
#include <util/delay.h>
#include <string.h>
#include <stdio.h>
#include <stdlib.h>
#include <avr/interrupt.h>

#include "uart0.h"				// 블루투스 통신
#include "Timer.h"				// 타이머 인터럽트
#include "mfrc522.h"			// RFID
#include "spi.h"				// RFID

#define ON						1
#define OFF						0
#define CW						1
#define CCW						0

#define SND_VALUE_SIZE			11
#define DELAY_TIME				256
// PORT
#define SENSOR_TRACCER_PORT		PINA
#define TRACER_MOTOR_PORT		PORTC
#define SENSOR_GOODS_PORT		PORTF
// Servo Motor
#define MOTOR_UPDOWN			OCR1A
#define MOTOR_GRIPPER			OCR1B
#define UP						1000
#define DOWN					3000
#define OPEN					1000
#define CLOSE					5000
// AGV 이동 상태
#define STOP					0
#define FORWARD					1
#define BACKWARD				2
// Gripper 작업 상태
#define INITIAL					0
#define PICK					1
#define DROP					2
// 제품 유무 상태
#define NOTHING					0
#define EXISTENCE				1
// AGV 도착 상태
#define PICKSTAGE				1
#define DROPSTAGE				2
#define PICKSTAY				3
#define DROPSTAY				4
// Gripper 의 pick or drop 완료 신호
#define SUCCESS					1
#define FAIL					2
// 시퀀스
int sequence					= 0;
// MES 에서 보낸 명령
int CMD_pick					= OFF;
int CMD_drop					= OFF;

// AGV -> MES로 전송될 데이터 저장 배열
int SND_value[SND_VALUE_SIZE];
// AGV에서 보낼 값들
int STATUS_move					= STOP;		// line-tracer move 상태 ( 0: stop, 1: forward, 2: backward )
int STATUS_gripper				= INITIAL;	// gripper 상태 ( 0: default, 1: pick, 2: drop )
int STATUS_goods				= NOTHING;	// 제품 유무 상태 ( 0: nothing, 1: existence )
int FLAG_arrive					= INITIAL;	// AGV 도착 상태 ( 0: default, 1: pick stage, 2: drop stage )
int FLAG_gripper_cplt			= INITIAL;	// gripper 의 pick or drop 완료 신호 ( 0: default, 1: success, 2: fail )
uint8_t RFID_tag_temp[MAX_LEN];				// RFID TAG 임시 저장 값
uint8_t RFID_tag_value[5];					// RFID TAG 값

// Flag
int FLAG_rfid					= 0;		// AGV가 pick stage or drop stage 에 도착하였는지 확인 신호

// pick, drop stage RFID TAG 값
uint8_t TAG_value_pickstage[5] = { 0x94, 0x9c, 0x1e, 0x1e, 0x08 };		// pick stage Tag 값 (임의 설정)
uint8_t TAG_value_dropstage[5] = { 0x87, 0xe0, 0x6e, 0x63, 0x6a };		// drop stage Tag 값 (임의 설정)
uint8_t TAG_value_pickstay[5]  = { 0x73, 0x09, 0x59, 0x64, 0x47 };		// pick  stay Tag 값 (임의 설정)
uint8_t TAG_value_dropstay[5]  = { 0x00, 0x00, 0x00, 0x00, 0x00 };		// drop  stay Tag 값 (임의 설정)

// receiveData 분할 위한 함수들
char *ptr_receiveData;						// receiveData를 ","로 분할하여 저장하는 포인터
char *cnt;									// receiveData를 분할한 값의 첫 번째 부분 ( 카운트 : 001, 002, ... )
char *cmd;									// receiveData를 분할한 값의 두 번째 부분 ( 명령어 : pick, drop, stat )
int ptr_cnt						= 0;		// 위의 receiveData를 저장 하기 위한 count 변수

volatile int cnt_event			= 0;		// 이벤트 발생 정보 MES에 전송 시 count 하기 위한 변수 (e01)

// 타이머
extern volatile int tmr_motor;				// motor 타이머
extern volatile int tmr_rfid;				// RFID  타이머
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
//extern volatile int tmr_sleep;				// sleep 타이머
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

unsigned char tracer_sensor;				// 라인트레이서 라인감지 센서
char buffer[50];							// sprintf 사용하기 위해 선언한 buffer 변수
uint8_t byte;								// RFID status 변수
volatile int delay;							// delay 변수
int cds_present = 0, cds_previous = 0;		// 조도센서

FILE OUTPUT = FDEV_SETUP_STREAM(UART0_transmit, NULL, _FDEV_SETUP_WRITE);
FILE INPUT = FDEV_SETUP_STREAM(NULL, UART0_receive, _FDEV_SETUP_READ);

ISR(USART0_RX_vect) {
	UART0_ISR_Receive();
}

void init_pwm(void)
{
	// 모드 14, 고속 PWM 모드
	TCCR1A |= (1 << WGM11);
	TCCR1B |= (1 << WGM12) | (1 << WGM13);
	
	// 비반전 모드
	TCCR1A |= (1 << COM1A1) | (1 << COM1B1);
	
	TCCR1B |= (1 << CS11);		// 분주율 8, 2MHz
	ICR1 = 39999;				// 20ms 주기
}

void init_adc(void) {
	ADCSRA |= (1 << ADPS2) | (1 << ADPS1) | (1 << ADPS0);		// 분주비 16M/128 = 125KH
	ADMUX |= (1 << REFS0);									// AVCC (5V)
	ADCSRA |= (1 << ADEN);									// ADC Enable
}

void init_port(void) {
	DDRB  |= 0x60;
	PORTC |= 0x00; //m103 output only
	DDRC  |= 0xff;
	PORTD |= 0x00;
	DDRD  |= 0x00;
	PORTE |= 0x00;
	DDRE  |= 0x00;
	PORTG |= 0x00;
	DDRG  |= 0x03;
}

void init_gripper(void) {
	OCR1A = 1000;		// UP
	OCR1B = 1100;		// 그리퍼 Open
}

///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// AGV sleep mode 설정
//void set_sleep_mode(int sleep_mode) {
//MCUCR = sleep_mode;
//return;
//}
//void sleep_mode_on(void) {
//MCUCR |= 0x20;
//return;
//}
//void sleep_mode_off(void) {
//MCUCR &= ~0x20;
//return;
//}
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////


///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Line Tracer 함수
void tracer_delay(int n)
{
	volatile int i,j;
	for(i=1;i<n;i++)
	{
		for(j=1;j<600;j++);
	}
}

void M1A(int onoff){
	if(onoff==ON)
	TRACER_MOTOR_PORT = TRACER_MOTOR_PORT|0x01;
	else
	TRACER_MOTOR_PORT = TRACER_MOTOR_PORT&0xFE;
}

void M1A_(int onoff){
	if(onoff==ON)
	TRACER_MOTOR_PORT = TRACER_MOTOR_PORT|0x02;
	else
	TRACER_MOTOR_PORT = TRACER_MOTOR_PORT&0xFD;
}

void M1B(int onoff){
	if(onoff==ON)
	TRACER_MOTOR_PORT = TRACER_MOTOR_PORT|0x04;
	else
	TRACER_MOTOR_PORT = TRACER_MOTOR_PORT&0xFB;
}

void M1B_(int onoff){
	if(onoff==ON)
	TRACER_MOTOR_PORT = TRACER_MOTOR_PORT|0x08;
	else
	TRACER_MOTOR_PORT = TRACER_MOTOR_PORT&0xF7;
}


void Motor1(int CWCCW){
	if(CWCCW==CW) {
		M1A(ON);
		M1A_(OFF);
	}
	else {
		M1A(OFF);
		M1A_(ON);
	}
}

void Motor2(int CWCCW){
	if(CWCCW==CW) {
		M1B(ON);
		M1B_(OFF);
	}
	else {
		M1B(OFF);
		M1B_(ON);
	}
}

void MOTORSTOP(void){
	M1B(OFF);
	M1B_(OFF);
	M1A(OFF);
	M1A_(OFF);
}
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////////////////////

void fnc_snd_value(void) {
	SND_value[0] = sequence;
	SND_value[1] = STATUS_move;
	SND_value[2] = STATUS_gripper;
	SND_value[3] = STATUS_goods;
	SND_value[4] = FLAG_gripper_cplt;
	SND_value[5] = FLAG_arrive;
	for(int i = 6; i < SND_VALUE_SIZE; i++) {
		SND_value[i] = RFID_tag_value[i-6];
	}
	
	for (int i = 0; i < SND_VALUE_SIZE; i++) {
		if(i == 0) {						// sequence
			sprintf(buffer, "%03d,", SND_value[i]);
		}
		else if (i >= 1 && i < 6) {			// status & flag
			sprintf(buffer, "%d,", SND_value[i]);
		}
		else if (i >= 6 && i < 10 ) {		// RFID
			sprintf(buffer, "%02x,", SND_value[i]);
		}
		else {								// RFID
			sprintf(buffer, "%02x", SND_value[i]);
		}
		UART0_print_string(buffer);
	}
	UART0_print_string("\r");
	
	return;
}

void snd_event_value(void) {
	sprintf(buffer, "e%02d,", cnt_event);
	UART0_print_string(buffer);
	fnc_snd_value();
	cnt_event++;
	if(cnt_event >= 100) {
		cnt_event = 0;
	}
	return;
}

int readADC(uint8_t channel) {			// 값을 읽어오는 함수
	ADMUX &= 0xF0;
	ADMUX |= channel;
	
	ADCSRA |= (1 << ADSC);				// 변환 시작
	while (ADCSRA & (1 << ADSC));		// 변환 완료 되기를 기다린다.
	
	return ADCW;						// ADC 값 반환
}

void RFID_tag_reading(void) {
	// RFID 값 RFID_TAG_temp 배열에 저장
	byte = mfrc522_request(PICC_REQALL,RFID_tag_temp);
	if (byte == CARD_FOUND)
	{
		for (int i = 0; i < MAX_LEN; i++) {
			RFID_tag_temp[i] = ' ';
		}
		byte = mfrc522_get_card_serial(RFID_tag_temp);		// RFID TAG값 배열에 저장
		
		for(int i = 0; i < 5; i++) {
			RFID_tag_value[i] = RFID_tag_temp[i];
		}
		if(tmr_rfid >= 256) {
			tmr_rfid = 0;
			snd_event_value();
		}
	}
}

int array_element_equal(uint8_t ary1[], uint8_t ary2[], int size) {
	int i, j, s1=0, s2=0;
	
	for(i = 0; i < size; i++) {
		for(j = 0; j < size; j++) {
			if( ary1[i] == ary2[j] ) {	// a에 대한 b의 각 원소가 같으면
				s1++;					// s1을 증가
				break;
			}
		}
	}
	for(i = 0; i < size; i++) {
		for(j = 0; j < size; j++) {
			if( ary2[i] == ary1[j] ) {	// b에 대한 a의 각 원소가 같으면
				s2++;					// s2를 증가
				break;
			}
		}
	}
	if (s1 == s2 && s1 == size) {		// s1과 s2의 증가한 값이 같고
		return 1;						// s의 값이 size와 같다면, match
	}
	else {
		return 0;
	}
}

int main(void)
{
	uint8_t *receiveData;
	
	stdout = &OUTPUT;			// File pointer 2
	stdin = &INPUT;				// File pointer 0

	UART0_INIT();
	INIT_TIMER0();
	spi_init();
	mfrc522_init();
	init_port();
	init_pwm();
	init_adc();
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	//set_sleep_mode(SLEEP_MODE_PWR_DOWN);
	///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
	sei();
	
	while (1)
	{
		if(isRxString())
		{
			receiveData = getRxString();
			ptr_receiveData = strtok(receiveData, ",");
			while(ptr_receiveData != NULL) {
				if(ptr_cnt == 0) {
					cnt = ptr_receiveData;
				}
				else if(ptr_cnt == 1) {
					cmd = ptr_receiveData;
				}
				ptr_receiveData = strtok(NULL, ",");
				ptr_cnt++;
			}
			ptr_cnt = 0;
			if(strncmp(cmd, "pick", sizeof("pick")) == 0) {
				UART0_print_string(cnt);
				UART0_print_string(",");
				fnc_snd_value();
				CMD_pick = ON;
			}
			if(strncmp(cmd, "drop", sizeof("drop")) == 0) {
				UART0_print_string(cnt);
				UART0_print_string(",");
				fnc_snd_value();
				CMD_drop = ON;
			}
			if(strncmp(cmd, "stat", sizeof("stat")) == 0) {
				UART0_print_string(cnt);
				UART0_print_string(",");
				fnc_snd_value();
			}
			///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
			//if(strncmp(cmd, "soff", sizeof("soff")) == 0) {
			//sleep_mode_off();
			//}
			///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
		}
		
		RFID_tag_reading();
		
		switch(sequence)
		{
			case 0:			// gripper 초기화
				init_gripper();
				tmr_motor = 0;
				sequence++;
				break;
			
			case 1:			// delay
				if(tmr_motor >= DELAY_TIME) {
					///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
					//tmr_sleep = 0;
					///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
					sequence++;
				}
				break;
			
			case 2:			// MES 에서 보내는 pick stage or drop stage 이동 명령 대기
				if(CMD_pick == ON || CMD_drop == ON) {
					sequence++;
				}
				///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
				//if(tmr_sleep >= 76800) {		// 10분 동안 명령이 없으면 파워 다운 모드
				//sleep_mode_on();
				//}
				///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
				break;
			
			case 3:			// AGV 상태 업데이트
				STATUS_move = FORWARD;			// AGV 전진 상태
				snd_event_value();
				sequence++;
				break;
			
			case 4:			// 라인 트레이서 동작
				tracer_sensor = SENSOR_TRACCER_PORT & 0x0F;
				switch(tracer_sensor) {
					case 0x0F:		// 전진
						Motor1(0);
						Motor2(1);
						tracer_delay(100);
						MOTORSTOP();
						tracer_delay(1);
						break;
					
					case 0x00:		// 정지
						break;
					
					case 0x0E:		// 극 CW
						Motor1(1);
						Motor2(1);
						tracer_delay(100);
						MOTORSTOP();
						tracer_delay(1);
						break;
					
					case 0x07:		// 극 CCW
						Motor1(0);
						Motor2(0);
						tracer_delay(100);
						MOTORSTOP();
						tracer_delay(1);
						break;
				
					case 0x08:		// CCW
					case 0x0C:
					case 0x0D:
						Motor2(1);
						tracer_delay(100);
						MOTORSTOP();
						tracer_delay(1);
						break;
					
					case 0x01:		// CW
					case 0x03:
					case 0x0B:
						Motor1(0);
						tracer_delay(100);
						MOTORSTOP();
						tracer_delay(1);
					
					default:
						break;
				}
				sequence++;
				break;
			
			case 5:			// RFID TAG값 비교
				if(CMD_pick == ON) {
					FLAG_rfid = array_element_equal(RFID_tag_value, TAG_value_pickstage, 5);	// 두 배열이 같으면 1, 다르면 0
				}
				else if(CMD_drop == ON) {
					FLAG_rfid = array_element_equal(RFID_tag_value, TAG_value_dropstage, 5);	// 두 배열이 같으면 1, 다르면 0
				}
			
				// AGV의 현재 위치가 pick stage or drop stage 인지 판단
				if(FLAG_rfid == ON) {				// AGV가 pick stage or drop stage 에 위치한 경우
					FLAG_rfid = OFF;				// flag 초기화
					MOTORSTOP();
					tracer_delay(1);
					sequence++;
				}
				else {								// AGV가 pick stage or drop stage 에 위치하지 않은 경우
					sequence--;						// AGV 계속해서 이동
				}
				break;
			
			case 6:			// AGV 정지 후 일정 시간 뒤 pick or drop 모드로 sequence 이동
				if(CMD_pick == ON) {
					if(delay >= DELAY_TIME) {
						delay = 0;
						CMD_pick = OFF;				// 명령 초기화
						STATUS_move = STOP;			// AGV 정지 상태
						FLAG_arrive = PICKSTAGE;	// pick stage 이동 완료 flag
						sequence = 100;				// pick 동작 모드 sequence 이동
					}
					delay++;
				}
				else if(CMD_drop == ON) {
					if(delay >= DELAY_TIME) {
						delay = 0;
						CMD_drop = OFF;				// 명령 초기화
						STATUS_move = STOP;			// AGV 정지 상태
						FLAG_arrive = DROPSTAGE;	// drop stage 이동 완료 flag
						sequence = 200;				// drop 동작 모드 sequence 이동
					}
					delay++;
				}
				break;
			
			///////////////////////
			// gripper pick 동작 //
			///////////////////////
			case 100:
				cds_previous = readADC(0);
				sprintf(buffer,"%d\r\n", cds_previous);
				UART0_print_string(buffer);
				sequence++;
				break;
				
			case 101:
				STATUS_gripper = PICK;				// gripper pick 상태 update
				snd_event_value();
				sequence++;
				break;
			
			case 102:		// down
				MOTOR_UPDOWN = DOWN;
				tmr_motor = 0;
				sequence++;
				break;
			
			case 103:		// delay
				if(tmr_motor >= DELAY_TIME) {
					sequence++;
				}
				break;
			
			case 104:		// gripper close
				MOTOR_GRIPPER = CLOSE;
				tmr_motor = 0;
				sequence++;
				break;
			
			case 105:		// delay
				if(tmr_motor >= DELAY_TIME) {
					sequence++;
				}
				break;
			
			case 106:		// up
				MOTOR_UPDOWN = UP;
				tmr_motor = 0;
				sequence++;
				break;
			
			case 107:		// delay
				if(tmr_motor >= DELAY_TIME) {
					sequence++;
				}
				break;
			
			case 108:
				cds_present = readADC(0);
				sprintf(buffer,"%d\r\n", cds_present);
				UART0_print_string(buffer);
				if((cds_previous - cds_present) >= 50) {
					SENSOR_GOODS_PORT = EXISTENCE;
				}
				else {
					SENSOR_GOODS_PORT = NOTHING;
				}
				sequence++;
				break;
				
			case 109:		// AGV 제품 유무 확인 (센서)
				if(SENSOR_GOODS_PORT == NOTHING) {				// pick 동작 완료 했으나, 상품을 집지 못하거나 놓친 경우
					sequence = 130;
				}
				else if(SENSOR_GOODS_PORT == EXISTENCE) {		// pick 동작 완료 및 정상적으로 상품이 있는 경우
					sequence++;
				}
				break;
			
			case 110:		// pick 동작 완료 및 정상적으로 상품이 있는 경우
				FLAG_gripper_cplt = SUCCESS;	// pick 동작 완료 Flag
				STATUS_gripper = INITIAL;		// gripper 상태 default
				STATUS_move = FORWARD;			// AGV 전진 상태
				FLAG_arrive = INITIAL;			// pick stage 도착 신호 초기화
				snd_event_value();
				sequence++;
				break;
			
			case 111:		// 라인 트레이서 동작
				tracer_sensor = SENSOR_TRACCER_PORT & 0x0F;
				switch(tracer_sensor) {
					case 0x0F:		// 전진
					Motor1(0);
					Motor2(1);
					tracer_delay(100);
					MOTORSTOP();
					tracer_delay(1);
					break;
				
					case 0x00:		// 정지
					break;
				
					case 0x0E:		// 극 CW
					Motor1(1);
					Motor2(1);
					tracer_delay(100);
					MOTORSTOP();
					tracer_delay(1);
					break;
				
					case 0x07:		// 극 CCW
					Motor1(0);
					Motor2(0);
					tracer_delay(100);
					MOTORSTOP();
					tracer_delay(1);
					break;
				
					case 0x08:		// CCW
					case 0x0C:
					case 0x0D:
					Motor2(1);
					tracer_delay(100);
					MOTORSTOP();
					tracer_delay(1);
					break;
				
					case 0x01:		// CW
					case 0x03:
					case 0x0B:
					Motor1(0);
					tracer_delay(100);
					MOTORSTOP();
					tracer_delay(1);
				
					default:
					break;
				}
				sequence++;
				break;
			
			case 112:		// RFID 동작 ( pick stay 장소인지 판단 )
				// AGV의 현재 위치가 pick stay 인지 판단
				FLAG_rfid = array_element_equal(RFID_tag_value, TAG_value_pickstay, 5);	// 두 배열이 같으면 1, 다르면 0
			
				// AGV의 현재 위치가 pick stay 이면,
				if(FLAG_rfid == ON) {
					FLAG_rfid = OFF;		// flag 초기화
					MOTORSTOP();
					tracer_delay(1);
					///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
					//tmr_sleep = 0;
					///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
					sequence = 2;			// 명령 대기 상태로 이동
				}
				// AGV의 현재 위치가 pick stay 가 아니면,
				else {
					sequence--;				// AGV 계속해서 이동
				}
				break;
			
			case 130:		// pick 동작 완료 했으나, 상품을 집지 못하거나 놓친 경우, 동작 실패 flag 전송
				FLAG_gripper_cplt = FAIL;		// pick 동작 실패 Flag
				snd_event_value();
				sequence++;
				break;
			
			case 131:		// pick restart 대기 및 pick 모드 재시작
				if(CMD_pick == ON) {			// 다시 pick 명령이 오면,
					CMD_pick = OFF;				// 명령 초기화
					init_gripper();				// gripper 초기화
					tmr_motor = 0;
					sequence++;
				}
				break;
			
			case 132:
				if(tmr_motor >= DELAY_TIME) {
					sequence = 100;
				}
				break;
			
			
			
			///////////////////////
			// gripper drop 동작 //
			///////////////////////
			
			case 200:
				STATUS_gripper = DROP;		// gripper drop 상태 update
				snd_event_value();
				sequence++;
				break;
			
			case 201:		// down
				MOTOR_UPDOWN = DOWN;
				tmr_motor = 0;
				sequence++;
				break;
			
			case 202:		// delay
				if(tmr_motor >= DELAY_TIME) {
					sequence++;
				}
				break;
			
			case 203:		// gripper open
				MOTOR_GRIPPER = OPEN;
				tmr_motor = 0;
				sequence++;
				break;
			
			case 204:		// delay
				if(tmr_motor >= DELAY_TIME) {
					sequence++;
				}
				break;
			
			case 205:		// up
				MOTOR_UPDOWN = UP;
				tmr_motor = 0;
				sequence++;
				break;
			
			case 206:		// delay
				if(tmr_motor >= DELAY_TIME) {
					sequence++;
				}
				break;
			
			case 207:		// AGV 제품 유무 확인 (센서)
				if(SENSOR_GOODS_PORT == NOTHING) {			// drop 동작을 완료하고, 정상적으로 제품을 놓은 경우
					sequence = 220;
				}
				else if(SENSOR_GOODS_PORT == EXISTENCE) {	// drop 동작을 완료하였으나, 제품이 남아있는 경우
					sequence = 210;
				}
				break;
			
			case 210:
				FLAG_gripper_cplt = FAIL;		// drop 동작 실패 Flag
				snd_event_value();
				sequence++;
				break;
			
			case 211:		// drop restart 대기 및 drop 모드 재시작
				if(CMD_drop == ON) {			// 다시 drop 명령이 오면,
					CMD_drop = OFF;				// 명령 초기화
					sequence = 200;				// drop sequence 로 이동하여 drop 모드 재시작
				}
				break;
			
			case 220:		// drop 동작 완료 시
				FLAG_gripper_cplt = SUCCESS;	// drop 동작 완료 Flag
				STATUS_gripper = INITIAL;		// gripper 상태 default
				STATUS_move = FORWARD;			// AGV 전진 상태
				FLAG_arrive = INITIAL;			// drop stage 도착 신호 초기화
				snd_event_value();
				sequence++;
				break;
			
			case 221:		// 라인 트레이서 동작
				tracer_sensor = SENSOR_TRACCER_PORT & 0x0F;
				switch(tracer_sensor) {
					case 0x0F:		// 전진
					Motor1(0);
					Motor2(1);
					tracer_delay(100);
					MOTORSTOP();
					tracer_delay(1);
					break;
				
					case 0x00:		// 정지
					break;
				
					case 0x0E:		// 극 CW
					Motor1(1);
					Motor2(1);
					tracer_delay(100);
					MOTORSTOP();
					tracer_delay(1);
					break;
				
					case 0x07:		// 극 CCW
					Motor1(0);
					Motor2(0);
					tracer_delay(100);
					MOTORSTOP();
					tracer_delay(1);
					break;
				
					case 0x08:		// CCW
					case 0x0C:
					case 0x0D:
					Motor2(1);
					tracer_delay(100);
					MOTORSTOP();
					tracer_delay(1);
					break;
				
					case 0x01:		// CW                         
					case 0x03:
					case 0x0B:
					Motor1(0);
					tracer_delay(100);
					MOTORSTOP();
					tracer_delay(1);
				
					default:
					break;
				}
				sequence++;
				break;
			
			case 222:		// RFID 동작 ( drop stay 장소인지 판단 )
				// AGV의 현재 위치가 drop stay 인지 판단
				FLAG_rfid = array_element_equal(RFID_tag_value, TAG_value_dropstay, 5);	// 두 배열이 같으면 1, 다르면 0
				// AGV의 현재 위치가 drop stay 이면,
				if(FLAG_rfid == ON) {
					FLAG_rfid = OFF;		// flag 초기화
					MOTORSTOP();
					tracer_delay(1);
					///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
					//tmr_sleep = 0;
					///////////////////////////////////////////////////////////////////////////////////////////////////////////////////
					sequence = 2;			// 명령 대기 상태로 이동
				}
				// AGV의 현재 위치가 drop stay 가 아니면,
				else {
					sequence--;				// AGV 계속해서 이동
				}
				break;
			
			default:
				printf("case default\r\n");
				break;
		}
	}
}