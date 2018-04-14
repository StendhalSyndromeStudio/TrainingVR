#ifndef AUDIO_IO_STREAM_V1A_H
#define AUDIO_IO_STREAM_V1A_H

#include <QObject>

#include <mutex>
#include "voice_chat/audio_io_stream.h"

namespace vc_1 {

  class AudioIoStream_v1a : public voice_chat::AudioIoStream
  {
    Q_OBJECT

    quint64             _bufferSize;
    mutable QByteArray  _buffer;
    mutable std::mutex  _bufferMutex;
  public:
    explicit AudioIoStream_v1a(QObject *parent = nullptr);
    ~AudioIoStream_v1a();

    // AudioIoStream interface
  public:
    virtual quint64 size() const;
    virtual quint64 bufferSize() const;
    virtual QByteArray read(quint64 size) const;
    virtual quint64 read(char *data, quint64 length) const;

  public slots:
    virtual quint64 write(const QByteArray &data);
    virtual quint64 write(const char *data, quint64 length);
    virtual QByteArray take(quint64 length);
    virtual quint64 take(char *data, quint64 length);
  };

}



#endif // AUDIO_IO_STREAM_V1A_H
