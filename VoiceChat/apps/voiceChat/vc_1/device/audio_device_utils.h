#ifndef AUDIO_DEVICE_UTILS_H
#define AUDIO_DEVICE_UTILS_H

#include <memory>
#include <functional>

#include <mutex>
#include <QThread>
#include <QAudioInput>
#include <QAudioOutput>
#include <QAudioFormat>
#include <QDataStream>

#include <voice_chat/audio_io_stream.h>

namespace vc_1 {

  class AudioDeviceUtils : public QThread
  {
    Q_OBJECT
  public:
    enum class Status
    {
      Starting = 0, Running, Error
    };

    enum class Mode
    {
      Input, Output
    };

    using Handler = std::function<bool()>;
  private:
    Status  _status;

    bool active;
    bool running;

    Handler handler;
    std::shared_ptr<voice_chat::AudioIoStream> stream;


    QAudioDeviceInfo  _device;
    QAudioFormat      _format;
    QAudioInput       *_audioInput;
    QAudioOutput      *_audioOutput;
    std::mutex        _inputMutex;
    std::mutex        _outputMutex;
    QIODevice         *ioDevice;
  public:
    AudioDeviceUtils(Mode mode, std::shared_ptr<voice_chat::AudioIoStream> stream);
    ~AudioDeviceUtils();
  public:
    void stop();
    Status status() const;

  protected:
    void run();
  private:
    bool inputHandler();
    bool outputHandler();
  };

}



#endif // AUDIO_DEVICE_UTILS_H
