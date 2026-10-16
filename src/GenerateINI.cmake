foreach(name MenuCameraCode MenuCameraCall ConfinerCode ConfinerAspectCall
    MapWidthCode MapLevelCreateCall MapLevelApplyCreateCall MapLevelApplyResizeCall MapLevelResizeCall
    MapWorldCreateCall MapWorldApplyCreateCall MapWorldApplyResizeCall MapWorldResizeCall
    TarotLayoutCode TarotLayoutJump)
    if(name STREQUAL "MenuCameraCode")
        set(section .text)
    elseif(name STREQUAL "ConfinerCode")
        set(section .CameraBoundsHook)
    else()
        set(section .${name})
    endif()
    execute_process(COMMAND "${OBJCOPY}" -O binary -j "${section}"
        "${BINARY_DIR}/menu-camera.elf" "${BINARY_DIR}/${name}.bin"
        COMMAND_ERROR_IS_FATAL ANY)
    file(READ "${BINARY_DIR}/${name}.bin" bytes HEX)
    string(REGEX REPLACE "(..)" "\\1 " bytes "${bytes}")
    string(STRIP "${bytes}" bytes)
    string(TOUPPER "${bytes}" ${name})
endforeach()

file(READ "${SOURCE_DIR}/src/SUWSF.ini.in" template)
string(CONFIGURE "${template}" configuration @ONLY)
file(WRITE "${BINARY_DIR}/SUWSF.ini" "${configuration}")
