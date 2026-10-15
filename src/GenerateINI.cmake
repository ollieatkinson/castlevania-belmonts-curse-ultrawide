foreach(name MenuCameraCode MenuCameraCall)
    if(name STREQUAL "MenuCameraCode")
        set(section .text)
    else()
        set(section .MenuCameraCall)
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
