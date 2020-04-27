/*
 * UART_0.c
 *
 * Created: 2020-01-09 오전 11:31:53
 *  Author: kccistc
 */ 
#define F_CPU 16000000UL
#include <avr/io.h>
#include <util/delay.h>

volatile uint8_t rxString[64]={0};   // Add DHT11 rxString[]-->2차원 배열 바꾸고
                                              //circular queue구조 변경 rxString[][]
volatile uint8_t rxReadyFlag=0;   // Add DHT11


void init_uart0()
{
   /* Set baud rate */
   UBRR0H = 0;     // UBRR1H   --> UBRR0H
   UBRR0L = 207;   // UBRR1L --> UBRR0H 9600 bps 모드로 설정
   /* Enable receiver and transmitter */
   
   // UCSR0A |= (1 << U2X0);
   UCSR0A |= _BV(U2X0); 
   UCSR0C |= 0x06;
   //UCSR0B |= _BV(RXEN0);
   //UCSR0B |= _BV(TXEN0);
   UCSR0B = (1<<RXEN0)|(1<<TXEN0)|(1<<RXCIE0); //RXCIE0 : RX complete Interrupt Enable
}

void UART0_transmit(char data)
{
   while ( !(UCSR0A & (1<<UDRE0)) );
   UDR0 = data;
}

unsigned char UART0_receive(void)
{
   while ( !(UCSR0A & (1<<RXC0)) );
   return UDR0;
}

void UART0_printf_string(char *str)
{
   for (int i=0; str[i]; i++)
   UART0_transmit(str[i]);
}

void UART0_print_1_byte_number(uint8_t n)
{
   char numString[4] = "0";
   int i, index = 0;
   if (n > 0)
   {
      for (i = 0; n != 0; i++)
      {
         numString[i] = n % 10 + '0';
         n = n / 10;
      }
      numString[i] = '\0';
      index = i - 1;
   }
   for (i = index; i >= 0; i--)
   {
      UART0_transmit(numString[i]);
   }
}
uint8_t isRxD()
{
   return (UCSR0A & (1<<7));
}

uint8_t isRxString()
{
   return rxReadyFlag;
}

uint8_t* getRxString()
{
   rxReadyFlag = 0;
   return rxString;
}

void UART0_ISR_Receive()
{
   static uint8_t head=0;
   volatile uint8_t data;
   
   data = UDR0;
   if (data == '\n' || data == '\r')
   {
      rxString[head] = '\0';
      head=0;
      rxReadyFlag = 1;
   }
   else
   {
      rxString[head] = data;
      head++;
   }
}