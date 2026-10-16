get_aspect = 0x182e5c5e0;
set_orthographic_size = 0x182e5e5e0;
set_enabled = 0x182e9ee20;
lens_aspect = 0x180b0d9d0;
screen_height = 0x182e73670;
get_parent = 0x182ec88f0;
set_width = 0x182ebd860;
generate_clues = 0x18082e7c0;
invoke_room_event = 0x1808b2db0;
string_new = 0x18037f530;
find_object = 0x182eb1fc0;
set_active = 0x182eb3000;
SECTIONS {
 /DISCARD/ : { *(.note*) *(.comment) }
 .text 0x18047ee10 : { *(.text) }
 ASSERT(SIZEOF(.text) <= 0x50, "Menu hook overlaps camera-boundary hook")
 .CameraBoundsHook 0x18047ee60 : { *(.CameraBoundsHook) }
 ASSERT(SIZEOF(.CameraBoundsHook) <= 0x20, "Camera-boundary hook overlaps map-width hook")
 .MapWidthCode 0x18047ee80 : { *(.MapWidthCode) }
 ASSERT(SIZEOF(.MapWidthCode) <= 0x40, "Map-width hook overlaps Tarot-layout hook")
 .TarotLayoutCode 0x18047eec0 : { *(.TarotLayoutCode) }
 ASSERT(SIZEOF(.TarotLayoutCode) <= 0x70, "Tarot-layout hook overlaps save-mask hook")
 .SaveMaskCode 0x18047ef30 : { *(.SaveMaskCode) }
 ASSERT(SIZEOF(.SaveMaskCode) <= 0xd0, "Save-mask hook exceeds unused executable padding")
 .SaveMaskJump 0x1808b37f7 : { *(.SaveMaskJump) }
 .TarotLayoutJump 0x18082f793 : { *(.TarotLayoutJump) }
 .MenuCameraCall 0x18086171d : { *(.MenuCameraCall) }
 .ConfinerAspectCall 0x1808d805c : { *(.ConfinerAspectCall) }
 .MapLevelCreateCall 0x18086ec2a : { *(.MapLevelCreateCall) }
 .MapLevelApplyCreateCall 0x18086b543 : { *(.MapLevelApplyCreateCall) }
 .MapLevelApplyResizeCall 0x18086b648 : { *(.MapLevelApplyResizeCall) }
 .MapLevelResizeCall 0x18086e299 : { *(.MapLevelResizeCall) }
 .MapWorldCreateCall 0x18087123e : { *(.MapWorldCreateCall) }
 .MapWorldApplyCreateCall 0x18086fcf7 : { *(.MapWorldApplyCreateCall) }
 .MapWorldApplyResizeCall 0x18086fdad : { *(.MapWorldApplyResizeCall) }
 .MapWorldResizeCall 0x180870da9 : { *(.MapWorldResizeCall) }
 .ArenaCameraCode 0x1831b4400 : { *(.ArenaCameraCode) }
 ASSERT(SIZEOF(.ArenaCameraCode) <= 0x60, "Arena hook exceeds reserved executable padding")
 .ArenaCameraCall 0x1807869c6 : { *(.ArenaCameraCall) }
 .CameraScreenRatioJump 0x180515407 : { *(.CameraScreenRatioJump) }
 .Confiner2DAspectCall 0x180ad1ef8 : { *(.Confiner2DAspectCall) }
}
