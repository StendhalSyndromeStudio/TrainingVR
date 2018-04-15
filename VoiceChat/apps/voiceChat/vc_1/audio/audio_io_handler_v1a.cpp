#include "audio_io_handler_v1a.h"
using namespace vc_1;

AudioIoHandler_v1a::AudioIoHandler_v1a(QObject *parent)
  : voice_chat::AudioIoHandler(parent)
  , _device ( new AudioDevice() )
{

}

AudioIoHandler_v1a::~AudioIoHandler_v1a()
{

}

bool vc_1::AudioIoHandler_v1a::isPlaying() const
{
  return _device->isPlaying();
}

bool vc_1::AudioIoHandler_v1a::isRecording() const
{
  return _device->isRecording();
}


double vc_1::AudioIoHandler_v1a::recLevel() const
{
  return 0.0;
}

double vc_1::AudioIoHandler_v1a::playLevel() const
{
  return 0.0;
}

voice_chat::AudioIoHandler::Merger AudioIoHandler_v1a::output() const
{
  return _output;
}

voice_chat::AudioIoHandler::Splitter AudioIoHandler_v1a::input() const
{
  return _input;
}

bool vc_1::AudioIoHandler_v1a::initilize()
{

  return true;
}

void vc_1::AudioIoHandler_v1a::free()
{

}

voice_chat::AudioIoHandler::Error vc_1::AudioIoHandler_v1a::startPlay() const
{
  if ( _device->play( _output ) )
  {
    return voice_chat::AudioIoHandler::Error::NoError;
  }

  return voice_chat::AudioIoHandler::Error::UnknowError;
}

voice_chat::AudioIoHandler::Error vc_1::AudioIoHandler_v1a::startRecord() const
{
  if ( _device->record( _input ) )
  {
    return voice_chat::AudioIoHandler::Error::NoError;
  }

  return voice_chat::AudioIoHandler::Error::UnknowError;
}

void vc_1::AudioIoHandler_v1a::stopPlaying() const
{
  _device->stopPlaying();
}

void vc_1::AudioIoHandler_v1a::stopRecording() const
{
  _device->stopRecording();
}
