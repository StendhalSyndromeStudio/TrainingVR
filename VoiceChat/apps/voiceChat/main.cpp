#include <QApplication>

#include "vc_1/ivoice_chat.h"
#include "vc_1/voice_chat.h"

int main(int argc, char *argv[])
{
  QApplication a(argc, argv);
  a.setApplicationName( "Voice chat" );
  a.setApplicationDisplayName( "Голосовой чат" );

  a.setQuitOnLastWindowClosed( false );

  auto *vc = new vc_1::VoiceChat();

  if ( !vc->initilize() )
  {

    vc->free();
    delete vc;

    return 1;
  }

  QObject::connect( vc, &vc_1::VoiceChat::closeApplication,
                    [vc, &a](){
    vc->free();
    delete vc;

    a.exit( 0 );
  });

  vc->start();

  return a.exec();
}
