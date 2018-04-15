QT       += core gui multimedia network xml

greaterThan(QT_MAJOR_VERSION, 4): QT += widgets

TARGET      = voiceChat
TEMPLATE    = app

DEFINES += QT_DEPRECATED_WARNINGS
#DEFINES += QT_DISABLE_DEPRECATED_BEFORE=0x060000

SOURCES += \
        main.cpp \
    vc_1/ivoice_chat.cpp \
    vc_1/voice_chat.cpp \
    vc_1/audio/audio_io_handler_v1a.cpp \
    vc_1/audio/audio_core_v1a.cpp \
    vc_1/audio/audio_io_stream_v1a.cpp \
    vc_1/audio/audio_stream_merger_v1a.cpp \
    vc_1/audio/audio_stream_splitter_v1a.cpp \
    vc_1/device/audio_device.cpp \
    vc_1/device/audio_device_utils.cpp

HEADERS += \
    vc_1/ivoice_chat.h \
    vc_1/voice_chat.h \
    vc_1/audio/audio_io_handler_v1a.h \
    vc_1/audio/audio_core_v1a.h \
    vc_1/audio/audio_io_stream_v1a.h \
    vc_1/audio/audio_stream_merger_v1a.h \
    vc_1/audio/audio_stream_splitter_v1a.h \
    vc_1/device/audio_device.h \
    vc_1/device/audio_device_utils.h

FORMS +=

include(libs.pri)

RESOURCES += \
    res/res.qrc
