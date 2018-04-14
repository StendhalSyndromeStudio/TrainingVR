QT              -= gui

TARGET          = voiceChatInterfaces
TEMPLATE        = lib
CONFIG          += staticlib

DEFINES += QT_DEPRECATED_WARNINGS
#DEFINES += QT_DISABLE_DEPRECATED_BEFORE=0x060000

SOURCES += \
        voicechatinterfaces.cpp \
    voice_chat/audio_core.cpp \
    voice_chat/audio_io_handler.cpp

HEADERS += \
        voicechatinterfaces.h \
    voice_chat/audio_core.h \
    voice_chat/audio_io_handler.h
