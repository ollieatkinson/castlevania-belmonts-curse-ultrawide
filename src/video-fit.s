.intel_syntax noprefix
.section .VideoFitCode,"ax"
.global video_fit
# Fit the shared video presenter instead of stretching it across the screen.
# VideoPlayer already uses FitInside for its render texture. Keep that behavior.
video_fit:
    push rbx
    push rsi
    push rdi
    sub rsp, 0x50
    mov rbx, rcx
    mov rsi, rdx
    call set_video_texture
    mov [rsp + 0x40], rax
    test rsi, rsi
    je done
    mov rcx, rsi
    xor edx, edx
    call texture_width
    test eax, eax
    jle done
    cvtsi2ss xmm0, eax
    movss [rsp + 0x30], xmm0
    mov rcx, rsi
    xor edx, edx
    call texture_height
    test eax, eax
    jle done
    cvtsi2ss xmm1, eax
    movss xmm0, [rsp + 0x30]
    divss xmm0, xmm1
    movss [rsp + 0x30], xmm0
    mov rcx, rbx
    xor edx, edx
    call get_transform
    test rax, rax
    je done
    mov rdi, rax
    mov rcx, rax
    xor edx, edx
    call get_parent
    test rax, rax
    je done
    mov rdx, rax
    lea rcx, [rsp + 0x20]
    xor r8d, r8d
    call get_rect
    # Width = min(parent width, parent height * texture aspect).
    movss xmm0, [rsp + 0x28]
    movss xmm1, [rsp + 0x2c]
    xorps xmm2, xmm2
    ucomiss xmm0, xmm2
    jbe done
    ucomiss xmm1, xmm2
    jbe done
    mulss xmm1, [rsp + 0x30]
    minss xmm0, xmm1
    movss [rsp + 0x34], xmm0
    divss xmm0, [rsp + 0x30]
    movss [rsp + 0x38], xmm0
    # The shipped presenter has a centered pivot and zero anchored position.
    mov rcx, rdi
    xor edx, edx
    movss xmm2, [rsp + 0x34]
    xor r9d, r9d
    call set_width
    mov rcx, rdi
    mov edx, 1
    movss xmm2, [rsp + 0x38]
    xor r9d, r9d
    call set_width
done:
    mov rax, [rsp + 0x40]
    add rsp, 0x50
    pop rdi
    pop rsi
    pop rbx
    ret

# Refresh after preparation and when the window or canvas changes size.
.global video_update
video_update:
    push rbx
    sub rsp, 0x30
    mov [rsp + 0x20], rcx
    mov [rsp + 0x28], rdx
    mov eax, [rcx + 0x8c]
    sub eax, 2
    cmp eax, 1
    ja resume_update
    mov rbx, [rcx + 0x28]
    test rbx, rbx
    je resume_update
    mov rcx, rbx
    xor edx, edx
    call get_video_texture
    test rax, rax
    je resume_update
    mov rdx, rax
    mov rcx, rbx
    xor r8d, r8d
    call video_fit
resume_update:
    mov rcx, [rsp + 0x20]
    mov rdx, [rsp + 0x28]
    jmp video_update_resume

.section .VideoFitCall,"a"
call video_fit

.section .VideoUpdateJump,"a"
jmp video_update
nop
