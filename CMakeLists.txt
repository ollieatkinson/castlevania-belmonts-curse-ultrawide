cmake_minimum_required(VERSION 3.24)
project(BelmontsCurseUltrawide VERSION 1.0.2 LANGUAGES NONE)

find_program(ASSEMBLER as REQUIRED)
find_program(LINKER ld REQUIRED)
find_program(OBJCOPY objcopy REQUIRED)

include(FetchContent)
FetchContent_Declare(suwsf
    URL https://github.com/PhantomGamers/SUWSF/releases/download/2.3.0/SUWSF-x64.zip
    URL_HASH SHA256=2f0831cba476f014819282e92b870156dbf964c8cc6d6412cd68040bee1612a0
    TLS_VERIFY TRUE
)
FetchContent_Declare(asi_loader
    URL https://github.com/ThirteenAG/Ultimate-ASI-Loader/releases/download/v9.7.4/Ultimate-ASI-Loader-NoPDB_x64.zip
    URL_HASH SHA256=e5860e7d9a1805267535b65749575b5e406cc6ea3325c7392189c578815045d1
    TLS_VERIFY TRUE
)
FetchContent_MakeAvailable(suwsf asi_loader)

add_custom_command(OUTPUT "${CMAKE_BINARY_DIR}/SUWSF.ini"
    COMMAND "${ASSEMBLER}" --64 -o menu-camera.o "${CMAKE_SOURCE_DIR}/src/menu-camera.s"
    COMMAND "${ASSEMBLER}" --64 -o camera-bounds.o "${CMAKE_SOURCE_DIR}/src/camera-bounds.s"
    COMMAND "${LINKER}" -T "${CMAKE_SOURCE_DIR}/src/menu-camera.ld" -e menu_camera_fit
        -o menu-camera.elf menu-camera.o camera-bounds.o
    COMMAND "${CMAKE_COMMAND}" "-DSOURCE_DIR=${CMAKE_SOURCE_DIR}" "-DBINARY_DIR=${CMAKE_BINARY_DIR}"
        "-DOBJCOPY=${OBJCOPY}" -P "${CMAKE_SOURCE_DIR}/src/GenerateINI.cmake"
    DEPENDS src/menu-camera.s src/camera-bounds.s src/menu-camera.ld src/SUWSF.ini.in src/GenerateINI.cmake
    WORKING_DIRECTORY "${CMAKE_BINARY_DIR}"
    VERBATIM
)
add_custom_target(configuration ALL DEPENDS "${CMAKE_BINARY_DIR}/SUWSF.ini")

install(FILES "${suwsf_SOURCE_DIR}/SUWSF.asi" "${CMAKE_BINARY_DIR}/SUWSF.ini" src/version.ini DESTINATION .)
install(FILES "${asi_loader_SOURCE_DIR}/dinput8.dll" DESTINATION . RENAME version.dll)
install(FILES README.md LICENSE THIRD_PARTY_NOTICES.md DESTINATION BelmontsCurseUltrawide)
install(DIRECTORY licenses DESTINATION BelmontsCurseUltrawide)

set(CPACK_GENERATOR ZIP)
set(CPACK_PACKAGE_FILE_NAME "${PROJECT_NAME}-v${PROJECT_VERSION}")
set(CPACK_PACKAGE_DIRECTORY "${CMAKE_SOURCE_DIR}/dist")
set(CPACK_INCLUDE_TOPLEVEL_DIRECTORY OFF)
set(CPACK_PACKAGE_CHECKSUM SHA256)
include(CPack)
