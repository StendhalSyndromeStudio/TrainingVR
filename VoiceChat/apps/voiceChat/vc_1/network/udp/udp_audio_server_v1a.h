#ifndef UDP_AUDIO_SERVER_V1A_H
#define UDP_AUDIO_SERVER_V1A_H

#include <QObject>
#include "udp_audio_server.h"

namespace vc_1 {

  class UdpAudioServerUtils;

  class UdpAudioServer_v1a : public UdpAudioServer
  {
    Q_OBJECT

    Splitter  _output;
    Merger    _input;

    UdpAudioServerUtils *_utils;

    QHash<QString, Destination> _destinations;
    QList<Destination>          _destinationsAdded;
    QList<Destination>          _destinationsRemoved;

    friend class UdpAudioServerUtils;
  public:
    explicit UdpAudioServer_v1a(QObject *parent = nullptr);
    ~UdpAudioServer_v1a();


    // UdpAudioServer interface
  public:
    virtual Splitter output() const override;
    virtual Stream input() const override;
    virtual QList<Destination> destination() const override;

  public slots:
    virtual void add(Destination destination) override;
    virtual void remove(Destination destination) override;
    virtual void clear() override;
  };


}


#endif // UDP_AUDIO_SERVER_V1A_H
