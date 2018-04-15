#ifndef AUDIO_DEVICE_H
#define AUDIO_DEVICE_H

#include <memory>
#include <QObject>
#include <voice_chat/audio_io_stream.h>

namespace vc_1 {

  class AudioDeviceUtils;

  class AudioDevice : public QObject
  {
    Q_OBJECT

    AudioDeviceUtils  *_inputUtils;
    AudioDeviceUtils  *_outputUtils;

    std::shared_ptr<voice_chat::AudioIoStream> _inputStream;
    std::shared_ptr<voice_chat::AudioIoStream> _outputStream;
  public:
    explicit AudioDevice(QObject *parent = nullptr);
    ~AudioDevice();
  public:
    bool isPlaying() const;
    bool isRecording() const;
  signals:
    ///
    /// \brief Обновлено состояние воспроизведения
    ///
    void plauingUpdateState(bool playing);
    ///
    /// \brief Обновлено состояние текущей записи
    /// \todo Изменить описание а то не корректно звучит
    ///
    void recordingUpdateState(bool recording);
  public slots:
    bool play(const std::shared_ptr<voice_chat::AudioIoStream> &stream);
    bool record(const std::shared_ptr<voice_chat::AudioIoStream> &stream);

    void stopPlaying();
    void stopRecording();
  };

}



#endif // AUDIO_DEVICE_H
