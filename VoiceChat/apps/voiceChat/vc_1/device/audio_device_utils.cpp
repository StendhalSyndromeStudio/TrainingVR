#include "audio_device_utils.h"
using namespace vc_1;

#include <thread>
#include <QDebug>

#include <voice_chat/audio_io_stream.h>

AudioDeviceUtils::AudioDeviceUtils(Mode mode, std::shared_ptr<voice_chat::AudioIoStream> stream)
  : QThread()
  , stream ( stream )
  , _audioInput ( nullptr )
  , _audioOutput ( nullptr )
{

  if ( mode == Mode::Input )
  {
    handler = std::bind(&AudioDeviceUtils::inputHandler, this);
  }
  else
  {
    handler = std::bind(&AudioDeviceUtils::outputHandler, this);
  }
}

AudioDeviceUtils::~AudioDeviceUtils()
{
  stop();
}

void AudioDeviceUtils::stop()
{
  active = false;

  do
  {
    std::this_thread::sleep_for(std::chrono::milliseconds(20));
  } while ( running );

  std::lock_guard<std::mutex> lockerInput( _inputMutex );
  std::lock_guard<std::mutex> lockerOutput( _outputMutex );
}

void AudioDeviceUtils::run()
{
  active = true;
  running = true;

  do
  {
    msleep( 5 );
  } while ( active &&  handler() );

  qDebug() << "Stoped";
  running = false;
}

bool AudioDeviceUtils::inputHandler()
{
  std::lock_guard<std::mutex> locker( _inputMutex );

  if ( !_audioInput )
  {
    _format.setSampleRate(48000);
    _format.setChannelCount(1);
    _format.setSampleSize(32);
    _format.setSampleType(QAudioFormat::Float);
    _format.setByteOrder(QAudioFormat::LittleEndian);
    _format.setCodec("audio/pcm");

    QAudioDeviceInfo info(QAudioDeviceInfo::defaultInputDevice());
    if (!info.isFormatSupported(_format)) {
      qWarning() << "Default format not supported - trying to use nearest";
      _format = info.nearestFormat(_format);
    }

    _audioInput = new QAudioInput( QAudioDeviceInfo::defaultInputDevice(), _format );
    ioDevice = _audioInput->start();

    if ( !ioDevice )
    {
      delete _audioInput;
      _audioInput = nullptr;

      return false;
    }

    msleep( 200 );
  }
  else
  {
    if ( _audioInput->bytesReady() )
    {
      stream->write( ioDevice->readAll() );
    }
  }

  return true;
}

bool AudioDeviceUtils::outputHandler()
{
  std::lock_guard<std::mutex> locker( _outputMutex );

  if ( !_audioOutput )
  {
    _format.setSampleRate(48000);
    _format.setChannelCount(1);
    _format.setSampleSize(32);
    _format.setSampleType(QAudioFormat::Float);
    _format.setByteOrder(QAudioFormat::LittleEndian);
    _format.setCodec("audio/pcm");

    QAudioDeviceInfo info(QAudioDeviceInfo::defaultOutputDevice());
    if (!info.isFormatSupported(_format)) {
      qWarning() << "Default format not supported - trying to use nearest";
      _format = info.nearestFormat(_format);
    }

    _audioOutput = new QAudioOutput( QAudioDeviceInfo::defaultOutputDevice(), _format );
    ioDevice = _audioOutput->start();

    if ( !ioDevice )
    {
      delete _audioOutput;
      _audioOutput = nullptr;

      return false;
    }

    msleep( 200 );
  }
  else
  {

    if ( stream->size() )
    {
      qDebug() << stream->size();
      ioDevice->write( stream->take( stream->size() ) );
    }

  }

  return true;
}
