#include "audio_io_stream_v1a.h"
using namespace vc_1;

#include <cmath>

#define lockBuffer() \
  std::lock_guard<std::mutex> locker(_bufferMutex)

AudioIoStream_v1a::AudioIoStream_v1a(QObject *parent)
  : voice_chat::AudioIoStream(parent)
{

}

AudioIoStream_v1a::~AudioIoStream_v1a()
{
  _buffer.clear();
}


quint64 vc_1::AudioIoStream_v1a::size() const
{
  lockBuffer();
  return _buffer.size();
}

quint64 vc_1::AudioIoStream_v1a::bufferSize() const
{
  return _bufferSize;
}

QByteArray vc_1::AudioIoStream_v1a::read(quint64 size) const
{
  lockBuffer();
  size = std::min(static_cast<int>(size), _buffer.length());

  return _buffer.mid(0, size);
}

quint64 vc_1::AudioIoStream_v1a::read(char *data, quint64 length) const
{
  lockBuffer();
  auto size = std::min(length, static_cast<quint64>( _buffer.length() ));
  std::memcpy(_buffer.data(), data, length);

  return size;
}

quint64 vc_1::AudioIoStream_v1a::write(const QByteArray &data)
{
  _buffer.append( data );

  if ( _buffer.length() )
  {
    _buffer = _buffer.mid( _buffer.size() - _bufferSize );
  }

  return data.size();
}

quint64 vc_1::AudioIoStream_v1a::write(const char *data, quint64 length)
{
  QByteArray temp(data, length);

  return write(temp);
}

QByteArray vc_1::AudioIoStream_v1a::take(quint64 length)
{
  auto data = read(length);

  lockBuffer();
  _buffer = _buffer.mid(data.length());

  return data;
}

quint64 vc_1::AudioIoStream_v1a::take(char *data, quint64 length)
{
  auto buf = read(data,length);

  lockBuffer();
  _buffer = _buffer.mid(length);

  return buf;
}


#undef lockBuffer
