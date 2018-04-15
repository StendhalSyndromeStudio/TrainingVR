#ifndef VOICE_CHAT_H
#define VOICE_CHAT_H

#include <QObject>
#include <QWidget>

#include <QSystemTrayIcon>

#include "ivoice_chat.h"

namespace vc_1
{

  class VoiceChat : public IVoiceChat
  {
    Q_OBJECT


    QSystemTrayIcon _tray;
  public:
    explicit VoiceChat(QObject *parent = nullptr);
    ~VoiceChat();


    // IVoiceChat interface
  public slots:
    virtual bool initilize();
    virtual void start();
    virtual void free();
  };

}

#endif // VOICE_CHAT_H
