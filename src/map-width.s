.intel_syntax noprefix
.section .MapWidthCode,"ax"
.global map_width
# Only map textures use this width; the destination panels are 16:9.
# Return round(Screen.height * 16 / 9), preserving Win64 stack alignment.
map_width:
    sub rsp, 0x28
    call screen_height
    shl eax, 4
    add eax, 4
    xor edx, edx
    mov ecx, 9
    div ecx
    add rsp, 0x28
    ret

.section .MapLevelCreateCall,"a"
call map_width
.section .MapLevelApplyCreateCall,"a"
call map_width
.section .MapLevelApplyResizeCall,"a"
call map_width
.section .MapLevelResizeCall,"a"
call map_width
.section .MapWorldCreateCall,"a"
call map_width
.section .MapWorldApplyCreateCall,"a"
call map_width
.section .MapWorldApplyResizeCall,"a"
call map_width
.section .MapWorldResizeCall,"a"
call map_width
