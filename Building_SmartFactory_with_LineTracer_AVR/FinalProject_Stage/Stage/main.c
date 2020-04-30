#define F_CPU 16000000UL

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

#define INITIAL					0
#define NORMAL					1
#define DEFECT					2

//Sensor, Motor Pin 설정
#define Sensor_Port				PINF
#define Sensor_Drop_AGV			0x01
#define Sensor_Drop_stage		0x02
#define Sensor_Buffer_stage		0x04
#define Sensor_Pick_stage		0x08
#define Sensor_Pick_AGV			0x10

#define Motor_Port				PORTB
#define Motor_Conv_Drop1		0x01
#define Motor_Conv_Drop2		0x02
#define Motor_Conv_Pick1		0x04
#define Motor_Conv_Pick2		0x08
#define Motor_Stopper			OCR1C

//Stopper 위치 설정
#define Pos_Stopper_Open		3000
#define Pos_Stopper_Throw		3900
#define Pos_Stopper_Close		5000

//Motor delay 설정
#define Delay_Stopper			64
#define Delay_Conv				64

int value[VALUE_SIZE]			= { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };
int comp_value[VALUE_SIZE]		= { -1, -1, -1, -1, -1, -1, -1, -1, -1, -1, -1 };

int Flag_AutoMode				= 0;				// 컨베이어 AutoMode ( 0: 수동 / 1: 자동 )
int Flag_ProdDefect				= 0;				// 제품 불량 판단 ( 1: 제품 정상 / 2: 제품 불량 )

int Motor_conv_drop_stat		= 0;				// Drop 컨베이어 작동 상태 ( 0: OFF / 1: ON )
int Motor_conv_pick_stat		= 0;				// Pick 컨베이어 작동 상태 ( 0: OFF / 1: ON )

int Drop_ready					= 0;
int Pick_ready					= 0;
int Motor_Stopper_open			= 0;

int Seqeunce					= 0;
int delay						= 0;

char *ptr_receiveData;								// receiveData를 ","로 분할하여 저장하는 포인터
char *cnt;											// receiveData를 분할한 값의 첫 번째 부분 ( 카운트 : 001, 002, ... )
char *cmd;											// receiveData를 분할한 값의 두 번째 부분 ( 명령어 : pick, drop, stat )
int ptr_cnt						= 0;				// 위의 receiveData를 저장 하기 위한 count 변수

int cnt_event					= 0;
int tmr							= 0;
char buffer[50]					= { };
	
FILE OUTPUT = FDEV_SETUP_STREAM(UART0_transmit, NULL, _FDEV_SETUP_WRITE);
FILE INPUT = FDEV_SETUP_STREAM(NULL, UART0_receive, _FDEV_SETUP_READ);

void init_timer(void)
{
	TCCR0 |= (1 << CS02) | (1 << CS01) | (1 << CS00);
	TIMSK |= (1 << TOIE0);
}

void init_pwm(void)
{
	TCCR1A |= (1 << WGM11);
	TCCR1B |= (1 << WGM12) | (1 << WGM13);
	
	TCCR1A |= (1 << COM1C1);
	TCCR1B |= (1 << CS11);
	ICR1 = 39999;
	
	OCR1C = Pos_Stopper_Close;
}

ISR(USART0_RX_vect)
{
	UART0_ISR_Receive();
}

ISR(TIMER0_OVF_vect)
{
	delay++;
	tmr++;
}

void Update_Value(void) 
{
	value[0]	 = Seqeunce;
	value[1]	 = (Sensor_Port&Sensor_Drop_AGV) == 0;
	value[2]	 = (Sensor_Port&Sensor_Drop_stage) == 0;
	value[3]	 = (Sensor_Port&Sensor_Buffer_stage) == 0;
	value[4]	 = (Sensor_Port&Sensor_Pick_stage) == 0;
	value[5]	 = (Sensor_Port&Sensor_Pick_AGV) == 0;
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
	for (int i = 0 ; i < 11 ; i++)
	{
		if(i == 0) {
			sprintf(buffer, "%03d,", value[i]);
			UART0_printf_string(buffer);
		}
		else if(i >= 1 && i < 10) {
			sprintf(buffer, "%d,", value[i]);
			UART0_printf_string(buffer);
		}
		else if(i == 10) {
			sprintf(buffer, "%d", value[i]);
			UART0_printf_string(buffer);
		}
	}
	UART0_printf_string("\r");
}

void Send_Event_Value(void)
{
	sprintf(buffer, "e%02d,", cnt_event);
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
	
	for(int i = 8; i < 11; i++) 
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

int Check_Pickable(void)
{
	if ((Sensor_Port & Sensor_Pick_stage) == Sensor_Pick_stage	// pick stage에 물건 x
		|| (Sensor_Port & Sensor_Pick_AGV) == 0					// pick stage에 agv o
		|| (Motor_Port & Motor_Conv_Pick1) == Motor_Conv_Pick1	// motor가 돌고있으면
		|| Motor_Stopper_open == 1)								// stopper가 열려있으면
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

int main(void)
{		
	DDRB |= (1 << PORTB0) | (1 << PORTB1) | (1 << PORTB2) | (1 << PORTB3);
	DDRB |= (1 << PORTB5) | (1 << PORTB6) | (1 << PORTB7);
	
	uint8_t *receiveData;
	
	init_uart0();
	init_timer();
	init_pwm();
	
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
			if(strncmp(cmd,"manu",4) == 0)
			{
				Flag_AutoMode = 0;
				UART0_printf_string(cnt);
				UART0_printf_string(",");
				Send_Value();
			}
			if(strncmp(cmd, "stat", 4) == 0)
			{
				UART0_printf_string(cnt);
				UART0_printf_string(",");
				Send_Value();
			}
			if(strncmp(cmd, "rset", 4) == 0)		// AVR Reset
			{
				PORTA = 0x00;
				PORTB = 0x00;
				PORTC = 0x00;
				PORTD = 0x00;
				PORTE = 0x00;
				asm("jmp 0");
			}
			if(strncmp(cmd, "pdok", 4) == 0)
			{
				Flag_ProdDefect = NORMAL;
			}
			if(strncmp(cmd, "pdno", 4) == 0)
			{
				Flag_ProdDefect = DEFECT;
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
		if ((Sensor_Port & Sensor_Pick_AGV) == Sensor_Pick_AGV &&							// Pick stage에 AGV가 없고
			(Sensor_Port & Sensor_Pick_stage) == 0 &&										// Pick stage에 물건이 있고
			(((Motor_Port & Motor_Conv_Pick1) | (Motor_Port & Motor_Conv_Pick2)) == 0))		// Pick Conv가 돌지 않으면
		{
			Pick_ready = 1;
		}
		else
		{
			Pick_ready = 0;
		}
		if ((Sensor_Port & Sensor_Drop_AGV) == Sensor_Drop_AGV &&							// Drop stage에 AGV가 없고
			(Sensor_Port & Sensor_Drop_stage) == Sensor_Drop_stage &&						// Drop stage에 물건이 없고
			(Sensor_Port & Sensor_Buffer_stage) == Sensor_Buffer_stage &&					// Buffer stage에 물건이 없고
			(((Motor_Port & Motor_Conv_Drop1) | (Motor_Port & Motor_Conv_Drop2)) == 0) &&	// Drop Conv가 돌지 않고
			(Motor_Stopper_open == 0))														// Stopper가 닫혀 있으면
		{
			Drop_ready = 1;
		}
		else
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
				if((Sensor_Port & Sensor_Drop_AGV) == Sensor_Drop_AGV && (Sensor_Port & Sensor_Drop_stage) == 0)
				{
					Seqeunce++;
				} 
			break;
			
			case 2:		// buffer sensor 들어올때까지 Drop Conv 구동
				if((Sensor_Port & Sensor_Buffer_stage) == Sensor_Buffer_stage)
				{
					tmr = 0;
					Motor_Port |= Motor_Conv_Drop1;
				}
				else if((Sensor_Port & Sensor_Buffer_stage) == 0)
				{
					if(tmr >= 32) {
						tmr = 0;
						Motor_Port &= ~Motor_Conv_Drop1;
						Seqeunce++;
					}
				}
			break;
			
			case 3:		// 제품 불량 판단
				if(Flag_ProdDefect != INITIAL)
				{
					if(Flag_ProdDefect == NORMAL)
					{
						Seqeunce++;
					}
					else if(Flag_ProdDefect == DEFECT)
					{
						delay = 0;
						Seqeunce = 10;
					}
				}
			break;
			
			case 4:		//Pick stage로 보내기 전에 확인(제품, AGV X)
				if((Sensor_Port & Sensor_Pick_stage) == Sensor_Pick_stage && (Sensor_Port & Sensor_Pick_AGV) == Sensor_Pick_AGV)
				{
					delay = 0;
					Seqeunce++;
				}
			break;
			
			
			case 5:		//Stopper open
				Motor_Stopper = Pos_Stopper_Open;
				Motor_Stopper_open = 1;
				if(delay >= Delay_Stopper)
				{
					Seqeunce++;
				}
			break;
			
			case 6:		//Buffer stage에서 Pick stage로 제품 보내기
				if((Sensor_Port & Sensor_Buffer_stage) == 0)
				{
					//buffer에 제품 있으므로 Drop, Pick Conv 계속 구동
					Motor_Port |= Motor_Conv_Drop1;
					Motor_Port |= Motor_Conv_Pick1;
					delay = 0;
				}
				else if((Sensor_Port & Sensor_Buffer_stage) == Sensor_Buffer_stage)
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
			
			case 7:		// Pick stage 감지 후 Pick Conv 정지
				if((Sensor_Port & Sensor_Pick_stage) == Sensor_Pick_stage)
				{
					//pick stage에 제품 도착 안했으므로 Pick Conv 계속 구동
				}
				else if((Sensor_Port & Sensor_Pick_stage) == 0)
				{
					//pick stage 제품 도착 완료, Pick Conv 정지
					Motor_Port &= ~Motor_Conv_Pick1;
					Seqeunce = 100;
				}
			break;
			
			case 10:	// 제품이 불량일 경우
				Motor_Stopper = Pos_Stopper_Throw;
				Motor_Stopper_open = 1;
				if(delay >= Delay_Stopper)
				{
					Seqeunce++;
				}
			break;
			
			case 11:	// 제품 버리기
				if((Sensor_Port & Sensor_Buffer_stage) == 0)
				{
					//buffer에 제품 있으므로 Drop, Pick Conv 계속 구동
					Motor_Port |= Motor_Conv_Drop1;
					delay = 0;
				}
				else if((Sensor_Port & Sensor_Buffer_stage) == Sensor_Buffer_stage)
				{
					//buffer에 제품 없어짐, Drop Conv 정지, Stopper close
					if (delay >= Delay_Conv)
					{
						Motor_Port &= ~Motor_Conv_Drop1;
						Motor_Stopper = Pos_Stopper_Close;
						Motor_Stopper_open = 0;
						
						Flag_ProdDefect = INITIAL;
						Drop_ready = 1;
						Seqeunce = 0;
					}
				}
			break;
			
			case 100:		//pick ready
				Flag_ProdDefect = INITIAL;
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

