#ifndef AUDIO_IO_HANDLER_H
#define AUDIO_IO_HANDLER_H

#include <memory>

#include <QObject>

#include <voice_chat/audio_io_handler.h>
#include "vc_1/device/audio_device.h"

#include "vc_1/audio/audio_stream_merger_v1a.h"
#include "vc_1/audio/audio_stream_splitter_v1a.h"

namespace vc_1 {

  class AudioIoHandler_v1a : public voice_chat::AudioIoHandler
  {
    Q_OBJECT

    std::shared_ptr<AudioDevice> _device;

    std::shared_ptr<AudioStreamSplitter_v1a>  _input;
    std::shared_ptr<AudioStreamMerger_v1a>    _output;
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

    virtual Merger output() const;
    virtual Splitter input() const;
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
