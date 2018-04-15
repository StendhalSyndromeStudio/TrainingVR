#include "udp_audio_server_v1a.h"
using namespace vc_1;

#include "vc_1/audio/audio_stream_merger_v1a.h"
#include "vc_1/audio/audio_stream_splitter_v1a.h"

#include <thread>
#include <QHash>
#include <QThread>
#include <QUdpSocket>
#include <QDataStream>

#include "vc_1/audio/audio_io_stream_v1a.h"

#include "opus.h"

namespace vc_1 {

  QString getOpusError(int errorCode)
  {
    switch (errorCode) {
      case OPUS_OK:

        return "No error.";
        break;
      case OPUS_BAD_ARG:

        return "One or more invalid/out of range arguments.";
        break;
      case OPUS_BUFFER_TOO_SMALL:

        return "Not enough bytes allocated in the buffer.";
        break;
      case OPUS_INTERNAL_ERROR:

        return "An internal error was detected.";
        break;
      case OPUS_INVALID_PACKET:

        return "The compressed data passed is corrupted.";
        break;
      case OPUS_UNIMPLEMENTED:

        return "Invalid/unsupported request number";
        break;
      case OPUS_INVALID_STATE:

        return "An encoder or decoder structure is invalid or already freed.";
        break;
      case OPUS_ALLOC_FAIL:

        break;
      default:
        break;
    }

    return "undef error ( " + QString::number ( errorCode ) + " )";
  }

  class UdpSocketUtil
  {
    QUdpSocket socket;

    QHostAddress address;
    quint16 port;

    QString id;

    quint64    head;
    QByteArray buffer;

    OpusDecoder *decoder;
    OpusEncoder *encoder;

    std::shared_ptr<voice_chat::AudioIoStream> _input;
    std::shared_ptr<voice_chat::AudioIoStream> _output;
  public:
    UdpSocketUtil(QHostAddress address, quint16 port, QString id)
      : address ( address )
      , port ( port )
      , id ( id )

      , head ( 0 )

      , _input ( new vc_1::AudioIoStream_v1a() )
      , _output ( new vc_1::AudioIoStream_v1a() )
    {
      socket.bind( address, port );

      socket.connectToHost( address, port );

      int decoderError = 0, encoderError = 0;

      decoder = opus_decoder_create ( 48000, 1, &decoderError );
      encoder = opus_encoder_create ( 48000, 1, OPUS_APPLICATION_AUDIO, &encoderError );

      if ( !decoder ) {

        qDebug() << "Can not create opus decoder: " << getOpusError ( decoderError );
      }

      if ( !encoder ) {

        qDebug() << "Can not create opus encoder: " << getOpusError ( encoderError );
      }

      if ( decoder && encoder ) {

        decoderError = opus_decoder_init ( decoder, 48000, 1 );
        encoderError = opus_encoder_init ( encoder, 48000, 1, OPUS_APPLICATION_AUDIO );

        if ( decoderError == OPUS_OK &&
             encoderError != OPUS_OK )
        {
          encoderError = opus_encoder_ctl ( encoder, OPUS_SET_BITRATE( 16000 ) );
          if ( encoderError != OPUS_OK ) {

            qDebug() << "Error set bitrate encoder";
          }
        }
      }


    }

    ~UdpSocketUtil()
    {
      if ( encoder )
      {
        opus_encoder_destroy(encoder);
        encoder = nullptr;
      }

      if ( decoder )
      {
        opus_decoder_destroy(decoder);
        encoder = nullptr;
      }
    }
  public:
    std::shared_ptr<voice_chat::AudioIoStream> input() const { return _input; }
    std::shared_ptr<voice_chat::AudioIoStream> output() const { return _output; }

    void handleRead()
    {
      auto size = 960 * 4;

      //      qDebug() << buffer.size();

//      socket.waitForReadyRead( 25 );
      if ( socket.hasPendingDatagrams() )
      {
        QByteArray data;
        data.resize( 960 );

        int readed = socket.readDatagram( data.data(), 960 );
        data.resize( readed );
        buffer.append( data );
      }

      while ( buffer.size() >= 16 )
      {
        if ( head == 0 )
        {
          while ( buffer.length() >= 16 )
          {
            bool ok = true;

//            qDebug() << buffer.mid( 0, 16 ).toHex();
            for ( int i = 0; i < 8; ++i )
            {
              if ( buffer[i] != '\xFF' )
              {
                ok = false;
                break;
              }
            }

            if ( ok )
            {
//              qDebug() << "find";
              break;
            }
            else
            {
              buffer = buffer.mid( 1 );
            }
          }

          if ( buffer.size() >= 16 )
          {
            QByteArray headData = buffer.mid( 8, 8 );
            buffer = buffer.mid( 16 );

            QDataStream out ( &headData, QIODevice::ReadOnly );
            out >> head;

            qDebug() << "head" << head;
          }

        }
        else if ( head <= buffer.size() )
        {
          QByteArray temp = buffer.mid( 0, head );
          buffer = buffer.mid( head );

          QByteArray audioData;
          audioData.resize( size );

          int decoded = opus_decode_float( decoder,
                                           reinterpret_cast<unsigned char *>( temp.data() ),
                                           temp.size(),
                                           reinterpret_cast<float *>( audioData.data() ),
                                           960,
                                           0 );
          if ( decoded > 0 )
          {
            audioData.resize( decoded );
            _input->write( temp );
          }

          qDebug() << "decoded" << decoded << "head" << head;
          head = 0;
        }
        else
        {
          std::this_thread::sleep_for( std::chrono::milliseconds( 50 ) );
          socket.waitForReadyRead();
          break;
        }
      }

    }
    void handleWrite()
    {
      auto size = 960 * 4;

      int count = 5;
      while ( _output->size() >= size && count-- > 0 )
      {
        QByteArray buffer;
        buffer.resize( size );

        auto temp = _output->take(size);

        int encoded = opus_encode_float( encoder,
                                         reinterpret_cast<float *>( temp.data() ),
                                         960,
                                         reinterpret_cast<unsigned char *>( buffer.data() ),
                                         buffer.size() );

        if ( encoded > 0 )
        {
          buffer.resize( encoded );

          QByteArray outData;
          QDataStream out ( &outData, QIODevice::WriteOnly );

          for ( int i = 0; i < 8; ++i )
          {
            out << static_cast<quint8>( '\xff' );
          }


          out << static_cast<quint64>( temp.size() );
          outData.append( temp );

//          qDebug() << "<< " << outData.toHex();

          socket.writeDatagram( outData, address, port );
        }
      }
    }

  };

  class UdpAudioServerUtils: public QThread
  {
    bool  active;
    bool  running;

    UdpAudioServer_v1a *server;
    QHash<QString, UdpSocketUtil *> sockets;
  public:
    UdpAudioServerUtils(UdpAudioServer_v1a *server)
      : QThread()
      , active ( false )
      , running ( false )
      , server (server)
    {

    }
    ~UdpAudioServerUtils()
    {
      stop();
    }
  public:
    void stop()
    {
      active = false;

      do
      {
        std::this_thread::sleep_for(std::chrono::milliseconds(20));
      } while ( running );
    }
  protected:
    void run()
    {
      active = true;
      running = true;

      do
      {
        while ( !server->_destinationsAdded.isEmpty() )
        {
          auto d = server->_destinationsAdded.takeFirst();

          QString id = QString( "%1:%2" )
              .arg( d.address.toString() )
              .arg( d.port );

          auto *s = new UdpSocketUtil( d.address, d.port, id );
          sockets[ id ] = s;
          server->_destinations[ id ] = d;

          server->_input->add( s->input() );
          server->_output->add( s->output() );
        }

        while ( !server->_destinationsRemoved.isEmpty() )
        {
          auto d = server->_destinationsRemoved.takeFirst();

          QString id = QString( "%1:%2" )
              .arg( d.address.toString() )
              .arg( d.port );

          if ( sockets.contains( id ) )
          {
            server->_destinations.remove( id );

            delete sockets[ id ];
            sockets.remove( id );
          }
        }

        for ( auto *s: sockets.values() )
        {
          s->handleWrite();
          s->handleRead();
        }

        msleep( 5 );
      } while ( active );

      qDebug() << "Stoped";
      running = false;
    }
  };

}



// ---------------------------------------------------------------------------

UdpAudioServer_v1a::UdpAudioServer_v1a(QObject *parent)
  : UdpAudioServer(parent)
  , _input ( new AudioStreamMerger_v1a() )
  , _output ( new AudioStreamSplitter_v1a() )
  , _utils  ( new UdpAudioServerUtils( this ) )
{
  _utils->start();
}

UdpAudioServer_v1a::~UdpAudioServer_v1a()
{
  if ( _utils )
  {
    _utils->stop();
    delete _utils;
  }
}


UdpAudioServer::Splitter vc_1::UdpAudioServer_v1a::output() const
{
  return _output;
}

vc_1::UdpAudioServer::Stream vc_1::UdpAudioServer_v1a::input() const
{
  return _input;
}

QList<vc_1::UdpAudioServer::Destination> vc_1::UdpAudioServer_v1a::destination() const
{
  return _destinations.values();
}

void vc_1::UdpAudioServer_v1a::add(Destination destination)
{
  _destinationsAdded.append( destination );
}

void vc_1::UdpAudioServer_v1a::remove(Destination destination)
{
  _destinationsRemoved.append( destination );
}

void vc_1::UdpAudioServer_v1a::clear()
{
  _destinationsRemoved = _destinations.values();
}
