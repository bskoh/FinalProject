/*
 * timer.c
 *
 * Created: 2020-01-30 오후 2:38:52
 *  Author: kccistc
 */ 
#include <avr/io.h>
#include <avr/interrupt.h>

extern volatile int Tmr_SND;			// 오버플로가 발생한 횟수
// 이곳 ISR(TIMER0_OVF_vect)   약16ms 마다 들어 온다.
ISR(TIMER0_OVF_vect)
{
	Tmr_SND++;
}

void init_timer0(void)
{
	// 분주비를 1024로 설정
	TCCR0 |= (1 << CS02) | (1 << CS01) | (1 << CS00);
	//  1024/16000000 = 256 / 16000 => 1/64 ==> 0.015625sec
	//  0.015625sec  ==> 16 ms 마다 over flow Int가 발생
	TIMSK |= (1 << TOIE0);			// 오버플로 인터럽트 허용

}

/*
ISR(TIMER1_COMPA_vect)
{
	fnd_timer_count++;    // 2ms마다 INT발생 하여 2ms마다 증가 된다.
	TCNT1 = 0;
}

void init_timer2(void)
{
	TCCR2 = 0b00000101;    // 분주비를 1024로 나눈다
	// 16M/1024 ==> 16000000/1024 ==> 15,625 HZ  ==> 15,625
	// T(1펄스의 시간) = 1/f ==> 1/15625Hz = 0.000064sec x 1000 = 0.064ms
	// 0.064 x 1000 = 64us가 소요
	// 그래서 TCN0 1이 64us 이므로 64를 곱해준것이다.
	// 1cm이동시 29us 소요
}
// 16bit timer1 OVF INT 분주비 1로 설정 4.096ms 마다 INT 발생
// FND 잔상 효과 유지 하기 위한 INT 
void init_timer1(void)
{
	OCR1A = 0x7FFF;    // 비교일치값 설정  7FFF ==> 32767
	TCCR1B |= (1 << CS10);    // 분주비를 1로 설정
	// TIMSK |= (1 << TOIE1);	  // 오버플로 인터럽트 	
	TIMSK |= (1 << OCIE1A);	  // 비교일치 A 인터럽트
}
*/