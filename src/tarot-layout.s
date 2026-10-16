.intel_syntax noprefix
.section .TarotLayoutCode,"ax"
.global tarot_layout
# Keep the card, clue list and description together at their original width.
tarot_layout:
    push rcx
    push rdx
    sub rsp, 0x28
    mov rcx, [rcx + 0x30]
    # Clue list -> list container -> selected view -> hunt view -> hunt section.
    .rept 4
    test rcx, rcx
    je restore
    xor edx, edx
    call get_parent
    mov rcx, rax
    .endr
    test rcx, rcx
    je restore
    xor edx, edx
    movss xmm2, [rip + layout_width]
    xor r9d, r9d
    call set_width
restore:
    add rsp, 0x28
    pop rdx
    pop rcx
    xor r8d, r8d
    jmp generate_clues
layout_width: .float 3740

.section .TarotLayoutJump,"a"
jmp tarot_layout
