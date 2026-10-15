.intel_syntax noprefix
.section .CameraBoundsHook,"ax"
.global confiner_aspect
# Bound the camera using its original 16:9 footprint. The real lens and
# projection keep their native aspect, so gameplay remains proportional.
confiner_aspect:
    sub rsp, 0x28
    call lens_aspect
    minss xmm0, [rip + normal_aspect]
    add rsp, 0x28
    ret
normal_aspect: .float 1.7777777777777778
.section .ConfinerAspectCall,"a"
call confiner_aspect
