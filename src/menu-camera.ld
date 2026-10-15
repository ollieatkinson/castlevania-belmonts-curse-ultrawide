get_aspect = 0x182e5bcb0;
set_orthographic_size = 0x182e5dcb0;
set_enabled = 0x182e9e4f0;
lens_aspect = 0x180b0d0a0;
SECTIONS {
 /DISCARD/ : { *(.note*) *(.comment) }
 .text 0x18047ee10 : { *(.text) }
 ASSERT(SIZEOF(.text) <= 0x50, "Menu hook overlaps camera-boundary hook")
 .CameraBoundsHook 0x18047ee60 : { *(.CameraBoundsHook) }
 ASSERT(SIZEOF(.CameraBoundsHook) <= 0x1a0, "Camera-boundary hook exceeds unused executable padding")
 .MenuCameraCall 0x180860ded : { *(.MenuCameraCall) }
 .ConfinerAspectCall 0x1808d772c : { *(.ConfinerAspectCall) }
}
