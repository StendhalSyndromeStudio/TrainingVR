#ifndef AUDIO_CORE_V1A_H
#define AUDIO_CORE_V1A_H

#include <QObject>

#include <voice_chat/audio_core.h>

namespace vc_1 {

  class AudioCore_v1a : public voice_chat::AudioCore
  {
    Q_OBJECT
  public:
    explicit AudioCore_v1a(QObject *parent = nullptr);
    ~AudioCore_v1a();

    // AudioCore interface
  public:
    virtual voice_chat::AudioIoHandler *audioHandler() const;
    virtual Mode mode() const;

  public slots:
    virtual bool initilize(Mode mode);
    virtual void free();
  };

}



#endif // AUDIO_CORE_V1A_H
