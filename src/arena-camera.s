.intel_syntax noprefix
.section .ArenaCameraCode,"ax"
.global arena_aspect
# Use the original footprint for custom rectangular camera bounds.
arena_aspect:
    sub rsp, 0x28
    call get_aspect
    minss xmm0, [rip + normal_aspect]
    add rsp, 0x28
    ret
normal_aspect: .float 1.7777777777777778

.section .ArenaCameraCall,"a"
call arena_aspect

# Preserve vertical coverage for cameras authored with a fixed width.
.section .CameraScreenRatioJump,"a"
jmp arena_aspect

# Apply the existing lens-aspect wrapper to the other confiner too.
.section .Confiner2DAspectCall,"a"
call confiner_aspect
