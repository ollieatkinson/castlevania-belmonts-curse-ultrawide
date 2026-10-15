.intel_syntax noprefix
.section .text
.global menu_camera_fit
# Called only when SwitchToWorldSpaceUi enables its menu camera.
# Keep the complete 3840x2160 menu visible at narrower aspect ratios.
menu_camera_fit:
    push rcx
    sub rsp, 0x20
    xor edx, edx
    call get_aspect
    movss xmm1, [rip + half_width]
    divss xmm1, xmm0
    maxss xmm1, [rip + half_height]
    mov rcx, [rsp + 0x20]
    xor r8d, r8d
    call set_orthographic_size
    add rsp, 0x20
    pop rcx
    mov edx, 1
    xor r8d, r8d
    jmp set_enabled
half_width: .float 1920
half_height: .float 1080
.section .MenuCameraCall,"a"
call menu_camera_fit
