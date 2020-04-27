/*
 * uart0.h
 *
 * Created: 2020-01-09 오전 11:22:54
 *  Author: Kwon Sun Seok
 */

#ifndef UART0_H_
#define UART0_H_

#define F_CPU 16000000UL
#include <avr/io.h>

void UART0_INIT(void);
void UART0_transmit(char data);
void UART0_ISR_Receive();
unsigned char UART0_receive(void);

void UART0_print_string(char *str);
void UART0_print_1_byte_number(uint8_t n);

uint8_t isRxD();
uint8_t isRxString();
uint8_t* getRxString();

#endif /* UART0_H_ */