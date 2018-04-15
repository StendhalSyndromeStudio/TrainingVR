#ifndef UDP_AUDIO_SERVER_H
#define UDP_AUDIO_SERVER_H

#include <QObject>
#include <QHostAddress>

#include <voice_chat/audio_stream_merger.h>
#include <voice_chat/audio_stream_splitter.h>

namespace vc_1 {

  class UdpAudioServer : public QObject
  {
    Q_OBJECT
  public:
    struct Destination
    {
      QHostAddress  address;
      quint16       port;
    };

    using Merger    = std::shared_ptr<voice_chat::AudioStreamMerger>;
    using Splitter  = std::shared_ptr<voice_chat::AudioStreamSplitter>;
    using Stream    = std::shared_ptr<voice_chat::AudioIoStream>;
  private:

  protected:
    explicit UdpAudioServer(QObject *parent = nullptr);
  public:
    ~UdpAudioServer();
  public:
    virtual Splitter output() const = 0;
    virtual Stream input() const = 0;

    virtual QList<Destination> destination() const = 0;
  signals:

  public slots:
    virtual void add(Destination destination) = 0;
    virtual void remove(Destination destination) = 0;
    virtual void clear() = 0;
  };

}



#endif // UDP_AUDIO_SERVER_H
