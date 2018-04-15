#include "audio_device.h"
using namespace vc_1;

#include "audio_device_utils.h"

AudioDevice::AudioDevice(QObject *parent)
  : QObject(parent)
  , _inputUtils ( nullptr )
  , _outputUtils ( nullptr )
{

}

AudioDevice::~AudioDevice()
{
  if ( isPlaying() )
    stopPlaying();

  if ( isRecording() )
    stopRecording();
}

bool AudioDevice::isPlaying() const
{
  return _outputUtils;
}

bool AudioDevice::isRecording() const
{
  return _inputUtils;
}

bool AudioDevice::play(const std::shared_ptr<voice_chat::AudioIoStream> &stream)
{
  stopPlaying();

  _outputStream = stream;

  _outputUtils = new AudioDeviceUtils( AudioDeviceUtils::Mode::Output, stream );
  _outputUtils->start();

  do
  {
    std::this_thread::sleep_for( std::chrono::milliseconds( 20 ) );
  } while ( _outputUtils->status() == AudioDeviceUtils::Status::Starting );

  bool ok = _outputUtils->status() == AudioDeviceUtils::Status::Running;
  emit plauingUpdateState( ok );

  return ok;
}

bool AudioDevice::record(const std::shared_ptr<voice_chat::AudioIoStream> &stream)
{
  stopRecording();

  _inputStream = stream;
  _inputUtils = new AudioDeviceUtils( AudioDeviceUtils::Mode::Input, stream );
  _inputUtils->start();

  do
  {
    std::this_thread::sleep_for( std::chrono::milliseconds( 20 ) );
  } while ( _inputUtils->status() == AudioDeviceUtils::Status::Starting );

  bool ok = _inputUtils->status() == AudioDeviceUtils::Status::Running;
  emit recordingUpdateState( ok );

  return ok;
}

void AudioDevice::stopPlaying()
{
  if ( _outputUtils )
  {
    _outputUtils->stop();

    delete _outputUtils;
    _outputUtils = nullptr;

    emit plauingUpdateState( false );
  }
}

void AudioDevice::stopRecording()
{
  if ( _inputUtils )
  {
    _inputUtils->stop();

    delete _inputUtils;
    _inputUtils = nullptr;

    emit recordingUpdateState( false );
  }
}
