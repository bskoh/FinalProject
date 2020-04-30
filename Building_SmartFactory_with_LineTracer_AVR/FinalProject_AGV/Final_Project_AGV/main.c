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
#define DELAY_TIME				128
// PORT
#define SENSOR_TRACCER_PORT		PINA
#define TRACER_MOTOR_PORT		PORTC
#define SENSOR_GOODS_PORT		PORTF
// Servo Motor
#define MOTOR_UPDOWN			OCR1A
#define MOTOR_GRIPPER			OCR1B
#define UP						1000
#define DOWN					2700
#define OPEN					1000
#define CLOSE					3500
// AGV 이동 상태
#define STOP					0
#define FORWARD					1
#define BACKWARD				2
// Gripper 작업 상태
#define INITIAL					0
#define PICK					1
#define DROP					2
// 제품 유무 상태
#define EMPTY					0
#define EXIST					1
// AGV 도착 상태
#define PICKSTAGE				1
#define DROPSTAGE				2
#define PICKSTAY				3
#define DROPSTAY				4
// Gripper 의 pick or drop 완료 신호
#define SUCCESS					1
#define FAIL					2

#define TRACER_DELAY_TIME1		27
#define TRACER_DELAY_TIME2		50
// 시퀀스
int sequence					= 0;

// MES 에서 보낸 명령
int CMD_pick					= OFF;
int CMD_drop					= OFF;

// AGV -> MES로 전송될 데이터 저장 배열
uint8_t SND_value[SND_VALUE_SIZE]	= { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
uint8_t COMP_value[SND_VALUE_SIZE]	= { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
	
// AGV에서 보낼 값들
uint8_t STATUS_move					= STOP;		// line-tracer move 상태 ( 0: stop, 1: forward, 2: backward )
uint8_t STATUS_gripper				= INITIAL;	// gripper 상태 ( 0: default, 1: pick, 2: drop )
uint8_t STATUS_goods				= EMPTY;	// 제품 유무 상태 ( 0: empty, 1: exist )
uint8_t FLAG_arrive					= INITIAL;	// AGV 도착 상태 ( 0: default, 1: pick stage, 2: drop stage )
uint8_t FLAG_work_cplt				= INITIAL;	// gripper 의 pick or drop 작업 완료 신호 ( 0: default, 1: success, 2: fail )
uint8_t RFID_tag_temp[MAX_LEN];					// RFID TAG 임시 저장 값
uint8_t RFID_tag_value[5];						// RFID TAG 값

// Flag
int FLAG_rfid						= 0;		// AGV가 pick stage or drop stage 에 도착하였는지 확인 신호
int FLAG_once						= 0;

// pick, drop stage RFID TAG 값
uint8_t TAG_value_pickstage[5]		= { 0x20, 0x99, 0xE1, 0x26, 0x7E };		// pick stage Tag 값 (임의 설정)
uint8_t TAG_value_dropstage[5]		= { 0xA2, 0xE3, 0x05, 0x30, 0x74 };		// drop stage Tag 값 (임의 설정)
uint8_t TAG_value_pickstay[5]		= { 0x45, 0xA5, 0xE5, 0x2B, 0x2E };		// pick  stay Tag 값 (임의 설정)
uint8_t TAG_value_dropstay[5]		= { 0xB6, 0xC0, 0x08, 0x25, 0x5B };		// drop  stay Tag 값 (임의 설정)

// receiveData 분할 위한 함수들
char *ptr_receiveData;						// receiveData를 ","로 분할하여 저장하는 포인터
char *cnt;									// receiveData를 분할한 값의 첫 번째 부분 ( 카운트 : 001, 002, ... )
char *cmd;									// receiveData를 분할한 값의 두 번째 부분 ( 명령어 : pick, drop, stat )
int ptr_cnt						= 0;		// 위의 receiveData를 저장 하기 위한 count 변수
	
// 이벤트 함수
volatile int cnt_event			= 0;		// 이벤트 발생 정보 MES에 전송 시 count 하기 위한 변수 (e01)

// 타이머
extern volatile int tmr_motor;				// motor 타이머
extern volatile int tmr;					// 타이머


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
	PORTF |= 0x00;
	DDRF  |= 0x00;
	PORTG |= 0x00;
	DDRG  |= 0x03;
}

void init_gripper_pick(void) {
	MOTOR_UPDOWN	= UP;
	MOTOR_GRIPPER	= OPEN;
}

void init_gripper_drop(void) {
	MOTOR_UPDOWN	= UP;
	MOTOR_GRIPPER	= CLOSE;
}


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

void Update_Value(void) {
	SND_value[0] = sequence;
	SND_value[1] = STATUS_move;
	SND_value[2] = STATUS_gripper;
	SND_value[3] = STATUS_goods;
	SND_value[4] = FLAG_arrive;
	SND_value[5] = FLAG_work_cplt;
	for(int i = 6; i < SND_VALUE_SIZE; i++) {
		SND_value[i] = RFID_tag_value[i-6];
	}
}

void fnc_snd_value(void) {
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


int Diff_Value(uint8_t ary1[], uint8_t ary2[])
{
	int count = 0;
	
	for(int i = 1; i < SND_VALUE_SIZE; i++)
	{
		if(ary1[i] != ary2[i])
		{
			count++;
			break;
		}
	}
	
	if (count != 0)
	{
		return 1;
	}
	else
	{
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
				CMD_drop = OFF;
			}
			if(strncmp(cmd, "drop", sizeof("drop")) == 0) {
				UART0_print_string(cnt);
				UART0_print_string(",");
				fnc_snd_value();
				CMD_drop = ON;
				CMD_pick = OFF;
			}
			if(strncmp(cmd, "stat", sizeof("stat")) == 0) {
				UART0_print_string(cnt);
				UART0_print_string(",");
				fnc_snd_value();
			}
			if(strncmp(cmd, "rset", sizeof("rset")) == 0) {		// AVR Reset
				PORTA = 0x00;
				PORTB = 0x00;
				PORTC = 0x00;
				PORTD = 0x00;
				PORTE = 0x00;
				asm("jmp 0");
			}
		}
		/////////////////////////////////////////////////////////////////////////////
		// RFID Tag값 상시 리딩
		RFID_tag_reading();
		/////////////////////////////////////////////////////////////////////////////
		// MES에 보내는 데이터 값 상시 업데이트
		Update_Value();
		/////////////////////////////////////////////////////////////////////////////
		// 데이터 값이 변할 시 Event 값으로 MES로 전송
		if(Diff_Value(SND_value, COMP_value)) {
			if(COMP_value[0] != -1) {					// 초기 부팅 시 제외
				snd_event_value();
			}
			for(int i = 1; i < SND_VALUE_SIZE; i++) {
				COMP_value[i] = SND_value[i];
			}
		}
		
		switch(sequence)
		{			
			case 0:			// MES 에서 보내는 pick stage or drop stage 이동 명령 대기
				if(CMD_pick == ON && CMD_drop == OFF) {
					init_gripper_pick();
					tmr_motor = 0;
					sequence++;
				}
				else if(CMD_pick == OFF && CMD_drop == ON) {
					init_gripper_drop();
					tmr_motor = 0;
					sequence++;
				}
				break;
			
			case 1:
				if(tmr_motor >= DELAY_TIME)	{
					FLAG_once = 0;
					sequence++;
				}
				break;
						
			case 2:			// RFID TAG값 비교
				if(CMD_pick == ON && CMD_drop == OFF) {
					FLAG_rfid = array_element_equal(RFID_tag_value, TAG_value_pickstage, 5);	// 두 배열이 같으면 1, 다르면 0
					if(FLAG_rfid == 1) {
						FLAG_arrive = PICKSTAGE;	// pick stage 이동 완료 flag
					}
				}
				else if(CMD_pick == OFF && CMD_drop == ON) {
					FLAG_rfid = array_element_equal(RFID_tag_value, TAG_value_dropstage, 5);	// 두 배열이 같으면 1, 다르면 0
					if(FLAG_rfid == 1) {
						FLAG_arrive = DROPSTAGE;
					}
				}
				
				// AGV의 현재 위치가 pick stage or drop stage 인지 판단
				if(FLAG_rfid == ON) {				// AGV가 pick stage or drop stage 에 위치한 경우
					MOTORSTOP();
					tracer_delay(1);
					STATUS_move = STOP;				// AGV 정지 상태로 업데이트
					
					tmr = 0;						// tmr 및 set flag 초기화
					FLAG_once = 0;
					sequence = 4;
				}
				else {								// AGV가 pick stage or drop stage 에 위치하지 않은 경우
					sequence++;						// AGV 계속해서 이동
				}
				break;
				
			case 3:			// 라인 트레이서 동작
					tracer_sensor = SENSOR_TRACCER_PORT & 0x0F;
					switch(tracer_sensor) {
						case 0x0F:					// 전진
							Motor1(0);
							Motor2(1);
							tracer_delay(TRACER_DELAY_TIME1);
							MOTORSTOP();
							tracer_delay(1);
							break;
								
						case 0x00:					// 정지
							break;
								
						case 0x0E:					// 극 CW
							Motor1(1);
							Motor2(1);
							tracer_delay(TRACER_DELAY_TIME1);
							MOTORSTOP();
							tracer_delay(1);
							break;
								
						case 0x07:					// 극 CCW
							Motor1(0);
							Motor2(0);
							tracer_delay(TRACER_DELAY_TIME1);
							MOTORSTOP();
							tracer_delay(1);
							break;
								
						case 0x08:					// CCW
						case 0x0C:
						case 0x0D:
							Motor2(1);
							tracer_delay(TRACER_DELAY_TIME1);
							MOTORSTOP();
							tracer_delay(1);
							break;
								
						case 0x01:					// CW
						case 0x03:
						case 0x0B:
							Motor1(0);
							tracer_delay(TRACER_DELAY_TIME1);
							MOTORSTOP();
							tracer_delay(1);
							break;
								
						default:
							break;
					}
					
					if(FLAG_once == 0) {			// AGV 전진 상태로 업데이트
						STATUS_move = FORWARD;
						FLAG_once++;
					}
					
					sequence--;
					break;
							
			case 4:			// AGV 정지 후 일정 시간 뒤 pick or drop 모드로 sequence 이동
				if(tmr >= DELAY_TIME) {
					tmr = 0;
					if(CMD_pick == ON && CMD_drop == OFF) {
						sequence = 100;				// pick 동작 모드 sequence
					}
					if(CMD_pick == OFF && CMD_drop == ON) {
						sequence = 200;				// drop 동작 모드 sequence
					}
				}
				break;
			
			///////////////////////
			// gripper pick 동작 //
			///////////////////////
			case 100:
				cds_previous = readADC(0);
				sequence++;
				break;
			
			case 101:		// down
				MOTOR_UPDOWN = DOWN;
				STATUS_gripper = PICK;				// gripper pick 상태 update
				tmr_motor = 0;
				sequence++;
				break;
			
			case 102:		// delay
				if(tmr_motor >= DELAY_TIME) {
					tmr_motor = 0;
					sequence++;
				}
				break;
			
			case 103:		// gripper close
				MOTOR_GRIPPER = CLOSE;
				sequence++;
				break;
			
			case 104:		// delay
				if(tmr_motor >= DELAY_TIME) {
					tmr_motor = 0;
					sequence++;
				}
				break;
			
			case 105:		// up
				MOTOR_UPDOWN = UP;
				sequence++;
				break;
			
			case 106:		// delay
				if(tmr_motor >= DELAY_TIME) {
					tmr_motor = 0;
					STATUS_gripper = INITIAL;		// gripper 상태 default
					sequence++;
				}
				break;
			
			case 107:			// AGV 제품 유무 확인 (센서)
				cds_present = readADC(0);
				if((cds_previous - cds_present) >= 50) {	// pick 동작 완료 및 정상적으로 상품이 있는 경우
					STATUS_goods = EXIST;
					sequence  = 110;
				}
				else {										// pick 동작 완료 했으나, 상품을 집지 못하거나 놓친 경우
					STATUS_goods = EMPTY;
					sequence = 130;
				}
				break;
			
			case 110:		// pick 동작 완료 및 정상적으로 상품이 있는 경우
				FLAG_work_cplt = SUCCESS;			// pick 동작 완료 Flag
				FLAG_once = 0;
				sequence++;
				break;
			
			case 111:		// RFID 동작 ( drop stay 장소인지 판단 )
				// AGV의 현재 위치가 drop stay 인지 판단
				FLAG_rfid = array_element_equal(RFID_tag_value, TAG_value_dropstay, 5);		// 두 배열이 같으면 1, 다르면 0
				
				// AGV의 현재 위치가 drop stay 이면,
				if(FLAG_rfid == ON) {
					MOTORSTOP();
					tracer_delay(1);
					STATUS_move = STOP;
					FLAG_work_cplt = INITIAL;
					
					FLAG_once = 0;					// Set Flag 초기화
					CMD_pick = OFF;					// 명령 초기화
					
					sequence = 0;					// 명령 대기 상태로 이동
				}
				// AGV의 현재 위치가 pick stay 가 아니면,
				else {
					sequence++;						// AGV 계속해서 이동
				}
				break;
				
			case 112:		// 라인 트레이서 동작
				tracer_sensor = SENSOR_TRACCER_PORT & 0x0F;
				switch(tracer_sensor) {
					case 0x0F:						// 전진
						Motor1(0);
						Motor2(1);
						tracer_delay(TRACER_DELAY_TIME2);
						MOTORSTOP();
						tracer_delay(1);
						break;
					
					case 0x00:						// 정지
						break;
					
					case 0x0E:						// 극 CW
						Motor1(1);
						Motor2(1);
						tracer_delay(TRACER_DELAY_TIME2);
						MOTORSTOP();
						tracer_delay(1);
						break;
					
					case 0x07:						// 극 CCW
						Motor1(0);
						Motor2(0);
						tracer_delay(TRACER_DELAY_TIME2);
						MOTORSTOP();
						tracer_delay(1);
								break;
					
					case 0x08:						// CCW
					case 0x0C:
					case 0x0D:
						Motor2(1);
						tracer_delay(TRACER_DELAY_TIME2);
						MOTORSTOP();
						tracer_delay(1);
						break;
					
					case 0x01:						// CW
					case 0x03:
					case 0x0B:
						Motor1(0);
						tracer_delay(TRACER_DELAY_TIME2);
						MOTORSTOP();
						tracer_delay(1);
						break;
					
					default:
						break;
				}
				
				if(FLAG_once == 0) {				// AGV 전진 상태로 업데이트
					STATUS_move = FORWARD;			// AGV 전진 상태
					FLAG_arrive = INITIAL;			// pick stage 도착 신호 초기화
					FLAG_once++;
				}
				
				sequence--;
				break;
			case 130:		// pick 동작 완료 했으나, 상품을 집지 못하거나 놓친 경우, 동작 실패 flag 전송
				FLAG_work_cplt = FAIL;				// pick 동작 실패 Flag
				CMD_pick = OFF;						// 명령 초기화
				
				sequence = 0;						// 명령 대기 상태로 이동
				break;
			
			
			
			///////////////////////
			// gripper drop 동작 //
			///////////////////////
			
			case 200:
				cds_previous = readADC(0);
				sequence++;
				break;
				
			case 201:		// down
				MOTOR_UPDOWN = DOWN;
				STATUS_gripper = DROP;				// gripper drop 상태 update
				tmr_motor = 0;
				sequence++;
				break;
			
			case 202:		// delay
				if(tmr_motor >= DELAY_TIME) {
					tmr_motor = 0;
					sequence++;
				}
				break;
			
			case 203:		// gripper open
				MOTOR_GRIPPER = OPEN;
				sequence++;
				break;
			
			case 204:		// delay
				if(tmr_motor >= DELAY_TIME) {
					tmr_motor = 0;
					sequence++;
				}
				break;
			
			case 205:		// up
				MOTOR_UPDOWN = UP;
				sequence++;
				break;
			
			case 206:		// delay
				if(tmr_motor >= DELAY_TIME) {
					tmr_motor = 0;
					STATUS_gripper = INITIAL;		// gripper 상태 default
					sequence++;
				}
				break;
				
			case 207:
				cds_present = readADC(0);
				if((cds_present - cds_previous) >= 30) {	// drop 동작 완료 및 정상적으로 상품이 없는 경우
					STATUS_goods = EMPTY;
					sequence = 210;
				}
				else {										// drop 동작 완료 했으나, 상품을 놓지 못한 경우
					STATUS_goods = EXIST;
					sequence = 220;
				}
				break;
			
			case 210:		// drop 동작 완료 시
				FLAG_work_cplt = SUCCESS;			// drop 동작 완료 Flag
				FLAG_once = 0;
				sequence++;
				break;
			
			case 211:		// RFID 동작 ( pick stay 장소인지 판단 )
				// AGV의 현재 위치가 pick stay 인지 판단
				FLAG_rfid = array_element_equal(RFID_tag_value, TAG_value_pickstay, 5);		// 두 배열이 같으면 1, 다르면 0
				// AGV의 현재 위치가 pick stay 이면,
				if(FLAG_rfid == ON) {
					MOTORSTOP();
					tracer_delay(1);
					STATUS_move = STOP;
					FLAG_work_cplt = INITIAL;
					
					FLAG_once = 0;					// Set Flag 초기화
					CMD_drop = OFF;					// 명령 초기화
					
					sequence = 0;					// 명령 대기 상태로 이동
				}
				// AGV의 현재 위치가 drop stay 가 아니면,
				else {
					sequence++;						// AGV 계속해서 이동
				}
				break;
			
			case 212:		// 라인 트레이서 동작
				tracer_sensor = SENSOR_TRACCER_PORT & 0x0F;
				switch(tracer_sensor) {
					case 0x0F:						// 전진
						Motor1(0);
						Motor2(1);
						tracer_delay(TRACER_DELAY_TIME2);
						MOTORSTOP();
						tracer_delay(1);
						break;
				
					case 0x00:						// 정지
						break;	
				
					case 0x0E:						// 극 CW
						Motor1(1);
						Motor2(1);
						tracer_delay(TRACER_DELAY_TIME2);
						MOTORSTOP();
						tracer_delay(1);
						break;
				
					case 0x07:						// 극 CCW
						Motor1(0);
						Motor2(0);
						tracer_delay(TRACER_DELAY_TIME2);
						MOTORSTOP();
						tracer_delay(1);
						break;
				
					case 0x08:						// CCW
					case 0x0C:
					case 0x0D:
						Motor2(1);
						tracer_delay(TRACER_DELAY_TIME2);
						MOTORSTOP();
						tracer_delay(1);
						break;
				
					case 0x01:						// CW
					case 0x03:
					case 0x0B:
						Motor1(0);
						tracer_delay(TRACER_DELAY_TIME2);
						MOTORSTOP();
						tracer_delay(1);
						break;
						
					default:
						break;
				}
			
				if(FLAG_once == 0) {
					STATUS_move = FORWARD;			// AGV 전진 상태
					FLAG_arrive = INITIAL;			// drop stage 도착 신호 초기화
					FLAG_once++;
				}
			
				sequence--;
				break;
			
			case 220:
				FLAG_work_cplt = FAIL;				// drop 동작 실패 Flag
				CMD_drop = OFF;						// 명령 초기화
				
				sequence = 0;						// 명령 대기 상태로 이동
				break;
				
			default:
				printf("case default\r\n");
				break;
		}
	}
}