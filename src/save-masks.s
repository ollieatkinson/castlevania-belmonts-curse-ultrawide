.intel_syntax noprefix
.section .SaveMaskCode,"ax"
.global save_masks
# After the room-entry event, hide only the two save-room side masks.
save_masks:
    sub rsp, 0x28
    call invoke_room_event
    lea rcx, [rip + left_mask]
    call hide_mask
    lea rcx, [rip + right_mask]
    call hide_mask
    add rsp, 0x28
    ret
hide_mask:
    sub rsp, 0x28
    call string_new
    mov rcx, rax
    xor edx, edx
    call find_object
    test rax, rax
    je done
    mov rcx, rax
    xor edx, edx
    xor r8d, r8d
    call set_active
done:
    add rsp, 0x28
    ret
left_mask: .asciz "Foreground_Saveroom_Black/00_Foreground_BlackSquare"
right_mask: .asciz "Foreground_Saveroom_Black/00_Foreground_BlackSquare (1)"

.section .SaveMaskJump,"a"
jmp save_masks
