/*
 * Timer.c
 *
 * Created: 2020-03-03 오후 5:06:02
 *  Author: kccistc
 */ 
#define F_CPU 16000000UL

#include <avr/io.h>
#include <avr/interrupt.h>

volatile int tmr_motor	= 0;
volatile int tmr		= 0;

ISR(TIMER0_OVF_vect) {
	tmr_motor++;
	tmr++;
}

void INIT_TIMER0(void) {
	TCCR0 |= (1 << CS02) | (1 << CS01) | (1 << CS00);	// 분주비 1,024 설정 ==> 64us
	TIMSK |= (1 << TOIE0);
}