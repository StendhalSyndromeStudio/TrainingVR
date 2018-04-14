#include "audio_stream_splitter_v1a.h"
using namespace vc_1;

AudioStreamSplitter_v1a::AudioStreamSplitter_v1a()
  : voice_chat::AudioStreamSplitter()
{

}

AudioStreamSplitter_v1a::~AudioStreamSplitter_v1a()
{

}


quint64 vc_1::AudioStreamSplitter_v1a::write(const QByteArray &data)
{
  std::lock_guard<std::mutex> locker( _outputMutex );

  for ( auto &s: _output )
  {
    s->write(data);
  }

  return data.size();
}

quint64 vc_1::AudioStreamSplitter_v1a::write(const char *data, quint64 length)
{
  std::lock_guard<std::mutex> locker( _outputMutex );

  for ( auto &s: _output )
  {
    s->write(data, length);
  }

  return length;
}

QList<voice_chat::AudioStreamSplitter::StreamPtr> vc_1::AudioStreamSplitter_v1a::output() const
{
  std::lock_guard<std::mutex> locker( _outputMutex );

  return _output;
}

int vc_1::AudioStreamSplitter_v1a::count() const
{
  std::lock_guard<std::mutex> locker( _outputMutex );

  return _output.count();
}

void vc_1::AudioStreamSplitter_v1a::add(const StreamPtr &stream)
{
  std::lock_guard<std::mutex> locker( _outputMutex );

  _output << stream;
}

void vc_1::AudioStreamSplitter_v1a::remove(const StreamPtr &stream)
{
  std::lock_guard<std::mutex> locker( _outputMutex );

  _output.removeOne( stream );
}

void vc_1::AudioStreamSplitter_v1a::removeAll()
{
  std::lock_guard<std::mutex> locker( _outputMutex );

  _output.clear();
}
