
#include <sm_api.h>
#include <string>

namespace sm
{
	class String
	{
	public:
		String( const std::string & text );
		String( smStringHandle stringHandle );

		const std::string & GetString( ) const;
		smStringHandle GetSmString( );

		int Length( ) const;
		const char * GetCString( ) const;

	private:
		std::string m_UsableString;
		smStringHandle m_SmString;
	};
}