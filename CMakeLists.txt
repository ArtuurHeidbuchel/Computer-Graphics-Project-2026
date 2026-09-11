cmake_minimum_required(VERSION 3.10)
project(F1_GraphicsProject)

set(CMAKE_CXX_STANDARD 17)
set(CMAKE_CXX_STANDARD_REQUIRED ON)

find_package(OpenGL REQUIRED)
find_package(glfw3 REQUIRED)
find_package(assimp REQUIRED)

include_directories(${PROJECT_SOURCE_DIR}/include)

add_executable(F1_Project
    glad.c
    src/main.cpp
    src/stb_imageDefinition.cpp
)

target_link_libraries(F1_Project PRIVATE
    glfw
    OpenGL::GL
    assimp::assimp
    ${CMAKE_DL_LIBS}
)