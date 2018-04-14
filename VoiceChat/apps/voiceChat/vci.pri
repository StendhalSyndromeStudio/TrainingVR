win32:CONFIG(release, debug|release): LIBS += -L$$OUT_PWD/../../libs/voiceChatInterfaces/release/ -lvoiceChatInterfaces
else:win32:CONFIG(debug, debug|release): LIBS += -L$$OUT_PWD/../../libs/voiceChatInterfaces/debug/ -lvoiceChatInterfaces
else:unix: LIBS += -L$$OUT_PWD/../../libs/voiceChatInterfaces/ -lvoiceChatInterfaces

INCLUDEPATH += $$PWD/../../libs/voiceChatInterfaces
DEPENDPATH += $$PWD/../../libs/voiceChatInterfaces

win32-g++:CONFIG(release, debug|release): PRE_TARGETDEPS += $$OUT_PWD/../../libs/voiceChatInterfaces/release/libvoiceChatInterfaces.a
else:win32-g++:CONFIG(debug, debug|release): PRE_TARGETDEPS += $$OUT_PWD/../../libs/voiceChatInterfaces/debug/libvoiceChatInterfaces.a
else:win32:!win32-g++:CONFIG(release, debug|release): PRE_TARGETDEPS += $$OUT_PWD/../../libs/voiceChatInterfaces/release/voiceChatInterfaces.lib
else:win32:!win32-g++:CONFIG(debug, debug|release): PRE_TARGETDEPS += $$OUT_PWD/../../libs/voiceChatInterfaces/debug/voiceChatInterfaces.lib
else:unix: PRE_TARGETDEPS += $$OUT_PWD/../../libs/voiceChatInterfaces/libvoiceChatInterfaces.a
