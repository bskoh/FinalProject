#define F_CPU 16000000L

#include <avr/io.h>
#include <util/delay.h>
#include <avr/interrupt.h>
#include <string.h>
#include <stdlib.h>
#include <stdio.h>

#include "uart0.h"

#define ON						1
#define OFF						0
#define	VALUE_SIZE				11

//Sensor, Motor Pin 설정
#define Sensor_Port				PINE
#define Sensor_Drop_AGV			0x01
#define Sensor_Drop_stage		1
#define Sensor_Buffer_stage		0x04
#define Sensor_Pick_stage		0x08
#define Sensor_Pick_AGV			0x10

#define Motor_Port				PORTB
#define Motor_Conv_Drop1		0x01
#define Motor_Conv_Drop2		0x02
#define Motor_Conv_Pick1		0x04
#define Motor_Conv_Pick2		0x08
#define Motor_Stopper			OCR1A

//Stopper 위치 설정
#define Pos_Stopper_Open		3000
#define Pos_Stopper_Close		1000

//Motor delay 설정
#define Delay_Stopper			128
#define Delay_Conv				64

int value[VALUE_SIZE] = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
int comp_value[VALUE_SIZE] = { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
	
int Flag_AutoMode = 0;


// CdS 밝을 때의 최소값
int CdS_high_drop_stage			= 1024;
int CdS_high_pick_stage			= 1024;
int CdS_high_buffer_stage		= 1024;
int CdS_high_drop_agv			= 1024;
int CdS_high_pick_agv			= 1024;

// CdS 어두울 때의 최대값
int CdS_low_drop_stage			= 0;
int CdS_low_pick_stage			= 0;
int CdS_low_buffer_stage		= 0;
int CdS_low_drop_agv			= 0;
int CdS_low_pick_agv			= 0;

int Motor_conv_drop_stat		= 0;
int Motor_conv_pick_stat		= 0;

int Drop_ready			= 0;
int Pick_ready			= 0;
int Motor_Stopper_open	= 0;

int Seqeunce			= 0;
int delay				= 0;

char *ptr_receiveData;		// receiveData를 ","로 분할하여 저장하는 포인터
char *cnt;					// receiveData를 분할한 값의 첫 번째 부분 ( 카운트 : 001, 002, ... )
char *cmd;					// receiveData를 분할한 값의 두 번째 부분 ( 명령어 : pick, drop, stat )
int ptr_cnt = 0;			// 위의 receiveData를 저장 하기 위한 count 변수

int cnt_event = 0;
char buffer[50] = { };

FILE OUTPUT = FDEV_SETUP_STREAM(UART0_transmit, NULL, _FDEV_SETUP_WRITE);
FILE INPUT = FDEV_SETUP_STREAM(NULL, UART0_receive, _FDEV_SETUP_READ);

void init_timer0(void)
{
	TCCR0 |= (1 << CS02) | (1 << CS01) | (1 << CS00);
	TIMSK |= (1 << TOIE0);
}

void init_timer1(void)
{
	TCCR1A |= (1 << WGM11);
	TCCR1B |= (1 << WGM12) | (1 << WGM13);
	
	TCCR1A |= (1 << COM1A1) | (1 << COM1B1) | (1 << COM1C1);
	TCCR1B |= (1 << CS11);
	ICR1 = 39999;
	
	OCR1A = 1000;
	OCR1B = 39999;
	OCR1C = 39999;
}

ISR(USART0_RX_vect)
{
	UART0_ISR_Receive();
}

ISR(TIMER0_OVF_vect)
{
	delay++;
}

void Update_Value(void) 
{
	value[0]	 = Seqeunce;
	value[1]	 = (PINE&0x01) == 0;
	value[2]	 = (PINE&0x02) == 0;
	value[3]	 = (PINE&0x04) == 0;
	value[4]	 = (PINE&0x08) == 0;
	value[5]	 = (PINE&0x10) == 0;
	// TODO : 모터 출력 1,0으로 나오게 하는 연산이 이 연산식이 맞는지 확인 Value 6, 7
	value[6]	 = ((Motor_Port & Motor_Conv_Drop1) | (Motor_Port & Motor_Conv_Drop2))
					== Motor_Conv_Drop1;
	value[7]	 = ((Motor_Port & Motor_Conv_Pick1) | (Motor_Port & Motor_Conv_Pick2))
					== Motor_Conv_Pick1;
	
	value[8]	 = Drop_ready;
	value[9]	 = Pick_ready;
	value[10]	 = Motor_Stopper_open;
}
void Send_Value(void)
{	
	for (int i = 1 ; i < 11 ; i++)
	{
		if(i < 10) {
			sprintf(buffer, "%d,", value[i]);
			UART0_printf_string(buffer);
		}
		else if(i == 10) {
			sprintf(buffer, "%d", value[i]);
			UART0_printf_string(buffer);
		}
	}
	UART0_printf_string("\n\r");
}

void Send_Event_Value(void)
{
	sprintf(buffer, "e%2d,", cnt_event);
	UART0_printf_string(buffer);
	Send_Value();
	cnt_event++;
	if(cnt_event >= 100)
	{
		cnt_event = 0;
	}
	return;
}

int Diff_Value(int ary1[], int ary2[]) 
{	
	int count = 0;
	
	for(int i = 6; i < 11; i++) 
	{
		if(ary1[i] != ary2[i])
		{
			count++;
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

int Check_Pickable(void)
{
	if ((Sensor_Port & Sensor_Pick_stage) == Sensor_Pick_stage	// pick stage에 물건 x
		|| (Sensor_Port & Sensor_Pick_AGV) == 0					// pick stage에 agv o
		|| (Motor_Port & Motor_Conv_Pick1) == Motor_Conv_Pick1	// motor가 돌고있으면
		|| Motor_Stopper_open == 1)								// stopper가 열려있으면						// agv의 위치가 pick stage가 아니면
	{
		return 0;
	}
	else
	{
		return 1;
	}
}
int Check_Dropable(void)
{
	if ((Sensor_Port & Sensor_Drop_stage) == 0					// drop stage에 물건 o
		|| (Sensor_Port & Sensor_Drop_AGV) == Sensor_Drop_AGV	// drop stage에 agv x
		|| (Motor_Port & Motor_Conv_Drop1) == Motor_Conv_Drop1	// motor가 돌고있으면
		|| Motor_Stopper_open == 1)								// stopper가 열려있으면
	{
		return 0;
	}
	else
	{
		return 1;
	}
}

//void CdS_Calibration(void) 
//{
	//int temp;
	//
	//UART0_printf_string("Calibration Start\r\n");
	//CdS_high_drop_stage = 1024;
	//for(int i = 0; i < 2048; i++) {
		//temp = readAdc(1);
		//if(temp < CdS_high_drop_stage) {
			//CdS_high_drop_stage = temp;
		//}
	//}
	//
	//CdS_low_drop_stage = CdS_high_drop_stage - 50;
	//
	//UART0_printf_string("Calibration End\r\n");
	//sprintf(buffer, "%d\r\n", CdS_high_drop_stage);
	//UART0_printf_string(buffer);
//}

int main(void)
{		
	DDRB |= (1 << PORTB0) | (1 << PORTB1) | (1 << PORTB2) | (1 << PORTB3);
	DDRB |= (1 << PORTB5) | (1 << PORTB6) | (1 << PORTB7);
	
	uint8_t *receiveData;
	
	UART0_Init();
	init_timer0();
	init_timer1();
	
	sei();
	
	while(1)
	{
		/////////////////////////////////////////////////////////////////////////////
		//MES에서 현재 AGV 위치 보내줌(Pick/Drop Stage일 경우)		
		if (isRxString())    // PC로 부터 수신된 Data가 존재 하는지 확인 있으면 아래를 수행 한다.
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
			
			if(strncmp(cmd,"auto",4) == 0)
			{
				Flag_AutoMode = 1;
				UART0_printf_string(cnt);
				UART0_printf_string(",");
				Send_Value();
			}
			else if(strncmp(cmd,"manu",4) == 0)
			{
				Flag_AutoMode = 0;
				UART0_printf_string(cnt);
				UART0_printf_string(",");
				Send_Value();
			}
			else if(strncmp(cmd, "stat", 4) == 0)
			{
				UART0_printf_string(cnt);
				UART0_printf_string(",");
				Send_Value();
			}
		}
		/////////////////////////////////////////////////////////////////////////////
		// MES에 보내는 데이터 값 상시 업데이트
		Update_Value();		
		
		/////////////////////////////////////////////////////////////////////////////
		// 데이터 값이 변할 시 Event 값으로 MES로 전송
		if(Diff_Value(value, comp_value) == 1)
		{
			if(comp_value[6] != -1)		// 초기 부팅 시 제외
			{
				Send_Event_Value();
			}
			for(int i = 6; i < VALUE_SIZE; i++)
			{
				comp_value[i] = value[i];
			}
		}
		
		/////////////////////////////////////////////////////////////////////////////
		// Pick, Drop ready 상태 초기화
		if (Check_Pickable() == 0)
		{
			Pick_ready = 0;
		}
		if (Check_Dropable() == 0)
		{
			Drop_ready = 0;
		}
				
		/////////////////////////////////////////////////////////////////////////////
		//Sequence		
		switch (Seqeunce)
		{
			case 0:		// seq start
				if (Flag_AutoMode == 1)
				{
					Seqeunce++;
				}
			break;
			
			case 1:		//Drop stage에 제품이 있고, AGV가 없을 때
				if(Check_Dropable() == 1)
				{
					Seqeunce++;
				} 
			break;
			
			case 2:		// buffer sensor 들어올때까지 Drop Conv 구동
				if((Sensor_Port & Sensor_Buffer_stage) == 0x00)
				{
					Motor_Port |= Motor_Conv_Drop1;
				}
				else if((Sensor_Port & Sensor_Buffer_stage) == Sensor_Buffer_stage)
				{
					Motor_Port &= ~Motor_Conv_Drop1;
					Seqeunce++;
				}
			break;
			
			case 3:		//Pick stage로 보내기 전에 확인(제품, AGV X)
				if((Sensor_Port & Sensor_Pick_stage) == 0x00 && (Sensor_Port & Sensor_Pick_AGV) == 0x00)
				{
					Seqeunce = 10;
					delay = 0;
				}
			break;
			
			case 10:	//Stopper open
				Motor_Stopper = Pos_Stopper_Open;
				Motor_Stopper_open = 1;
				if(delay >= Delay_Stopper)
				{
					Seqeunce++;
				}
			break;
			
			case 11:		//Buffer stage에서 Pick stage로 제품 보내기
				if((Sensor_Port & Sensor_Buffer_stage) == Sensor_Buffer_stage)
				{
					//buffer에 제품 있으므로 Drop, Pick Conv 계속 구동
					Motor_Port |= Motor_Conv_Drop1;
					Motor_Port |= Motor_Conv_Pick1;
					delay = 0;
				}
				else if((Sensor_Port & Sensor_Buffer_stage) == 0x00)
				{
					//buffer에 제품 없어짐, Drop Conv 정지, Stopper close
					if (delay >= Delay_Conv)
					{
						Motor_Port &= ~Motor_Conv_Drop1;
						
						Motor_Stopper = Pos_Stopper_Close;
						Motor_Stopper_open = 0;
						Drop_ready = 1;
						
						Seqeunce++;
					}
				}
			break;
			
			case 12:		//Pick stage 감지 후 Pick Conv 정지
				if((Sensor_Port & Sensor_Pick_stage) == 0x00)
				{
					//pick stage에 제품 도착 안했으므로 Pick Conv 계속 구동
				}
				else if((Sensor_Port & Sensor_Pick_stage) == Sensor_Pick_stage)
				{
					//pick stage 제품 도착 완료, Pick Conv 정지
					Motor_Port &= ~Motor_Conv_Pick1;
					Seqeunce = 100;
				}
			break;
			
			case 100:		//pick ready
				Pick_ready = 1;
				Seqeunce = 0;
			break;
						
			default:			
			break;
		}
		/////////////////////////////////////////////////////////////////////////////
	}

	return 0;
}

