/*
* uart0.c
*
* Created: 2020-01-09 오전 11:22:37
*  Author: Kwon Sun Seok
*/

#define F_CPU 16000000UL
#include <avr/io.h>
#include <util/delay.h>

volatile uint8_t rxString[64] = {0};
volatile uint8_t rxReadyFlag = 0;

void UART0_INIT(void) {
	/* Set baud rate */
	UBRR0H = 0;
	UBRR0L = 207;   // 9600 bps 모드로 설정
	
	// 비동기, 8비트 데이터, 패리티 없음, 1비트 정지 비트 모드
	UCSR0A |= (1 << U2X0);
	UCSR0C |= 0x06;
	UCSR0B = (1<<RXEN0) | (1<<TXEN0) | (1<<RXCIE0);  // Receive, Transmit 가능 및 Receive Complete Interrupt Enable
}

void UART0_transmit(char data) {
	while( !(UCSR0A & (1 << UDRE0)) );	// 송신 가능 대기
	UDR0 = data;						// 데이터 전송
}

unsigned char UART0_receive(void) {
	while( !(UCSR0A & (1 << RXC0)) );	// 데이터 수신 대기
	return UDR0;
}

void UART0_print_string(char *str) {	// 문자열 송신
	for(int i = 0; str[i]; i++)			// '\0'문자를 만날 때까지 반복
	UART0_transmit(str[i]);				// 바이트 단위 출력
}

void UART0_ISR_Receive() {
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

void UART0_print_1_byte_number(uint8_t n) {
	char numString[4] = "0";
	int i, index = 0;
	
	if(n > 0)
	{
		for(i = 0; n != 0; i++)
		{
			numString[i] = n % 10 + '0';
			n = n / 10;
		}
		numString[i] = '\0';
		index = i - 1;
	}
	for(i = index; i >= 0; i--)		// 변환된 문자열을 역순으로 출력
	UART0_transmit(numString[i]);
}

uint8_t isRxD() {
	return (UCSR0A & (1<<7));
}

uint8_t isRxString() {
	return rxReadyFlag;
}

uint8_t* getRxString() {
	rxReadyFlag = 0;
	return rxString;
}