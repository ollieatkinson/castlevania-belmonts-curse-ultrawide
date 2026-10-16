get_aspect = 0x182e5bcb0;
set_orthographic_size = 0x182e5dcb0;
set_enabled = 0x182e9e4f0;
lens_aspect = 0x180b0d0a0;
screen_height = 0x182e72d40;
get_parent = 0x182ec7fc0;
set_width = 0x182ebcf30;
generate_clues = 0x18082e280;
SECTIONS {
 /DISCARD/ : { *(.note*) *(.comment) }
 .text 0x18047ee10 : { *(.text) }
 ASSERT(SIZEOF(.text) <= 0x50, "Menu hook overlaps camera-boundary hook")
 .CameraBoundsHook 0x18047ee60 : { *(.CameraBoundsHook) }
 ASSERT(SIZEOF(.CameraBoundsHook) <= 0x20, "Camera-boundary hook overlaps map-width hook")
 .MapWidthCode 0x18047ee80 : { *(.MapWidthCode) }
 ASSERT(SIZEOF(.MapWidthCode) <= 0x40, "Map-width hook overlaps Tarot-layout hook")
 .TarotLayoutCode 0x18047eec0 : { *(.TarotLayoutCode) }
 ASSERT(SIZEOF(.TarotLayoutCode) <= 0x140, "Tarot-layout hook exceeds unused executable padding")
 .TarotLayoutJump 0x18082f253 : { *(.TarotLayoutJump) }
 .MenuCameraCall 0x180860ded : { *(.MenuCameraCall) }
 .ConfinerAspectCall 0x1808d772c : { *(.ConfinerAspectCall) }
 .MapLevelCreateCall 0x18086e2fa : { *(.MapLevelCreateCall) }
 .MapLevelApplyCreateCall 0x18086ac13 : { *(.MapLevelApplyCreateCall) }
 .MapLevelApplyResizeCall 0x18086ad18 : { *(.MapLevelApplyResizeCall) }
 .MapLevelResizeCall 0x18086d969 : { *(.MapLevelResizeCall) }
 .MapWorldCreateCall 0x18087090e : { *(.MapWorldCreateCall) }
 .MapWorldApplyCreateCall 0x18086f3c7 : { *(.MapWorldApplyCreateCall) }
 .MapWorldApplyResizeCall 0x18086f47d : { *(.MapWorldApplyResizeCall) }
 .MapWorldResizeCall 0x180870479 : { *(.MapWorldResizeCall) }
}
