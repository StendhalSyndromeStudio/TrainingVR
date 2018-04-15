#ifndef AUDIO_STREAM_SPLITTER_H
#define AUDIO_STREAM_SPLITTER_H

#include <memory>

#include "audio_types.h"
#include "audio_io_stream.h"

namespace voice_chat {

  class AudioStreamSplitter: public AudioIoStream
  {
    Q_OBJECT
  public:
    using StreamPtr = std::shared_ptr<AudioIoStream>;
  public:
    AudioStreamSplitter();
  public:
    virtual QList<StreamPtr> output() const = 0;
    virtual int count() const = 0;
  public:
    virtual void add(const StreamPtr &stream) = 0;
    virtual void remove(const StreamPtr &stream) = 0;
    virtual void removeAll() = 0;

    // AudioIoStream interface
  public:
    virtual quint64 size() const;
    virtual quint64 bufferSize() const;
    virtual QByteArray read(quint64 size) const;
    virtual quint64 read(char *data, quint64 length) const;

  public slots:
    virtual QByteArray take(quint64 length);
    virtual quint64 take(char *data, quint64 length);
  };

}



#endif // AUDIO_STREAM_SPLITTER_H
