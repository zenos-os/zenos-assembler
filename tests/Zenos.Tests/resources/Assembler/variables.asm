bits 64

section .data
HSP: dq 256
LCL: dq 1024
ARG: dq 2048
THIS: dq 4096
THAT: dq 0

section .text

	; D=17
	mov rdx, 17

	; *SP=D
	mov rax, [HSP]
	mov [rax], rdx

	; SP++
	mov rax, [HSP]
	add rax, 1
	mov [HSP], rax