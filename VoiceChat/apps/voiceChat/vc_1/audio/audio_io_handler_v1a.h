#ifndef AUDIO_IO_HANDLER_H
#define AUDIO_IO_HANDLER_H

#include <QObject>

#include "voice_chat/audio_io_handler.h"

namespace vc_1 {

  class AudioIoHandler_v1a : public voice_chat::AudioIoHandler
  {
    Q_OBJECT
  protected:
    explicit AudioIoHandler_v1a(QObject *parent = nullptr);
  public:
    virtual ~AudioIoHandler_v1a();

    // AudioIoHandler interface
  public:
    virtual bool isPlaying() const;
    virtual bool isRecording() const;
    virtual double recLevel() const;
    virtual double playLevel() const;

  public slots:
    virtual bool initilize();
    virtual void free();
    virtual Error startPlay() const;
    virtual Error startRecord() const;
    virtual void stopPlaying() const;
    virtual void stopRecording() const;
  };

}



#endif // AUDIO_IO_HANDLER_H
