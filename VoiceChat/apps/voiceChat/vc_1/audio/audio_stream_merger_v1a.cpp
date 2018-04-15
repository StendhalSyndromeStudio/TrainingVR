#include "audio_stream_merger_v1a.h"
using namespace vc_1;

#include <thread>
#include <utility>
#include <QThread>
#include <QDateTime>


//----------------- AudioStreamMergerUtils ----------------------

AudioStreamMergerUtils::AudioStreamMergerUtils(AudioStreamMerger_v1a *merger)
  : QThread()
  , merger (merger)
{

}

AudioStreamMergerUtils::~AudioStreamMergerUtils()
{

}

void AudioStreamMergerUtils::stop()
{
  active = false;

  do
  {
    std::this_thread::sleep_for(std::chrono::milliseconds(20));
  } while ( running );
}

void AudioStreamMergerUtils::run()
{
  active = true;
  running = true;

  auto freq = 48000;
  auto time = 0.5;

  auto size         = static_cast<quint64>( freq * time );
  auto readDataSize = 960 * 8; //static_cast<quint64>( size * sizeof(float) );

  auto *data        = new float[ size ];

  QList<voice_chat::AudioStreamMerger::StreamPtr> temp;

  QDateTime dt;
  do
  {
    dt = QDateTime::currentDateTime();

    {
      std::lock_guard<std::mutex> inLocker( merger->_inputMutex );

      for ( auto &s: merger->_input )
      {
        if ( s->size() >= readDataSize )
        {
          temp << s;
        }
        else
        {
          printf( "%d \n", s->size() );
        }
      }

      if ( !temp.isEmpty() )
      {
        auto fd = temp.first();
        voice_chat::AudioIoStream::take<float>( fd.get(), data, size );

        if ( !temp.isEmpty() )
        {
          auto *tempData = new float[ size ];

          while ( !temp.isEmpty() )
          {
            voice_chat::AudioIoStream::take<float>( temp.takeFirst().get(), tempData, size );

            for ( int i = 0; i < size; ++i )
            {
              data[ i ] = data[ i ] + tempData[ i ];
            }
          }

          if ( merger->_output )
          {
            std::lock_guard<std::mutex> outputLocker( merger->_outputMutex );
            voice_chat::AudioIoStream::write<float>(merger->_output.get(), data, size );
          }

          delete []tempData;
        }
      }
    }

    int sleep = (time * 0.85) - dt.msecsTo( QDateTime::currentDateTime() );
    while ( sleep > 0 )
    {
      std::this_thread::sleep_for( std::chrono::milliseconds( 5 ) );
      sleep -= 5;
    }
  }
  while ( active );

  delete []data;

  running = false;
}


//--------------- Base class ------------------------------

#include "vc_1/audio/audio_io_stream_v1a.h"
AudioStreamMerger_v1a::AudioStreamMerger_v1a()
  : _utils ( new AudioStreamMergerUtils( this ) )
  , _output ( new vc_1::AudioIoStream_v1a() )
{

  _utils->start();
}

AudioStreamMerger_v1a::~AudioStreamMerger_v1a()
{

}


voice_chat::AudioFormat vc_1::AudioStreamMerger_v1a::format() const
{
  return _format;
}

QList<voice_chat::AudioStreamMerger::StreamPtr> vc_1::AudioStreamMerger_v1a::input() const
{
  return _input;
}

int vc_1::AudioStreamMerger_v1a::count() const
{
  return _input.count();
}

void vc_1::AudioStreamMerger_v1a::setFormat(voice_chat::AudioFormat format)
{
  std::lock_guard<std::mutex> inLocker( _inputMutex );
  std::lock_guard<std::mutex> outLocker( _outputMutex );

  _format = format;
}

void vc_1::AudioStreamMerger_v1a::add(const voice_chat::AudioStreamMerger::StreamPtr &stream)
{
  std::lock_guard<std::mutex> inLocker( _inputMutex );

  _input << stream;
}

void vc_1::AudioStreamMerger_v1a::remove(const voice_chat::AudioStreamMerger::StreamPtr &stream)
{
  std::lock_guard<std::mutex> inLocker( _inputMutex );

  _input.removeOne( stream );
}

void AudioStreamMerger_v1a::removeAll()
{
  std::lock_guard<std::mutex> inLocker( _inputMutex );
  _input.clear();
}


quint64 vc_1::AudioStreamMerger_v1a::size() const
{
  return _output->size();
}

quint64 vc_1::AudioStreamMerger_v1a::bufferSize() const
{
  return _output->bufferSize();
}

QByteArray vc_1::AudioStreamMerger_v1a::read(quint64 size) const
{
  return _output->read(size);
}

quint64 vc_1::AudioStreamMerger_v1a::read(char *data, quint64 length) const
{
  return _output->read(data, length);
}

QByteArray vc_1::AudioStreamMerger_v1a::take(quint64 length)
{
  return _output->take(length);
}

quint64 vc_1::AudioStreamMerger_v1a::take(char *data, quint64 length)
{
  return _output->take(data,length);
}
