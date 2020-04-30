/*
 * uart0.h
 *
 * Created: 2020-01-09 오전 11:02:25
 *  Author: kccistc
 */ 

#ifndef UART0_H_
#define UART0_H_
void init_uart0();
void UART0_transmit(char data);
unsigned char UART0_receive(void);
void UART0_printf_string(char *str);
void UART0_print_1_byte_number(uint8_t n);
uint8_t isRxD();
uint8_t isRxString();     
uint8_t* getRxString();   
void UART0_ISR_Receive(); 
#endif /* UART0_H_ */