get_aspect = 0x182e5bcb0;
set_orthographic_size = 0x182e5dcb0;
set_enabled = 0x182e9e4f0;
SECTIONS {
 .text 0x18047ee10 : { *(.text) }
 ASSERT(SIZEOF(.text) <= 0x1f0, "Menu hook exceeds unused executable padding")
 .MenuCameraCall 0x180860ded : { *(.MenuCameraCall) }
}
