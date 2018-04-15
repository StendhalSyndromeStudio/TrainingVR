#ifndef AUDIO_STREAM_MERGER_H
#define AUDIO_STREAM_MERGER_H

#include <memory>
#include <QObject>

#include "audio_types.h"
#include "audio_io_stream.h"

namespace voice_chat {

  class AudioStreamMerger: public voice_chat::AudioIoStream
  {
  public:
    using StreamPtr = std::shared_ptr<AudioIoStream>;
  protected:
    AudioStreamMerger();
  public:
    virtual ~AudioStreamMerger();
  public:
    ///
    /// \brief Формат аудио файла
    ///
    virtual AudioFormat format() const = 0;

    ///
    /// \brief Список стримов для слияния
    ///
    virtual QList<StreamPtr> input() const = 0;

    ///
    /// \brief Колличество стримов для слияния
    ///
    virtual int count() const = 0;
  public:
    virtual void setFormat(AudioFormat format) = 0;
    virtual void add(const StreamPtr &stream) = 0;
    virtual void remove(const StreamPtr &stream) = 0;
    virtual void removeAll() = 0;

    // AudioIoStream interface
  public slots:
    virtual quint64 write(const QByteArray &data);
    virtual quint64 write(const char *data, quint64 length);
  };


}


#endif // AUDIO_STREAM_MERGER_H
