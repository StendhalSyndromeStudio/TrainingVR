#ifndef AUDIO_STREAM_SPLITTER_V1A_H
#define AUDIO_STREAM_SPLITTER_V1A_H

#include <mutex>
#include <QObject>
#include <voice_chat/audio_stream_splitter.h>

namespace vc_1 {

  class AudioStreamSplitter_v1a : public voice_chat::AudioStreamSplitter
  {
    Q_OBJECT

    QList<StreamPtr>    _output;
    mutable std::mutex  _outputMutex;
  public:
    explicit AudioStreamSplitter_v1a();
    ~AudioStreamSplitter_v1a();

    // AudioIoStream interface
  public slots:
    virtual quint64 write(const QByteArray &data);
    virtual quint64 write(const char *data, quint64 length);

    // AudioStreamSplitter interface
  public:
    virtual QList<StreamPtr> output() const;
    virtual int count() const;
    virtual void add(const StreamPtr &stream);
    virtual void remove(const StreamPtr &stream);
    virtual void removeAll();
  };


}


#endif // AUDIO_STREAM_SPLITTER_V1A_H
