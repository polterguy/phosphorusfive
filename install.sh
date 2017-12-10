#!/bin/bash

clear

# First giving user some information about what this script actually does.

echo "================================================================================"
echo "Automatic installation script for Phosphorus Five"
echo "Please let it finish, without interruptions, which might take some time."
echo ""
echo "The software is distributed in the hope that it will be useful,"
echo "but WITHOUT ANY WARRANTY; without even the implied warranty of"
echo "MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the"
echo "GNU General Public License for more details."
echo "================================================================================"
echo ""

# Making sure we update and upgrade our server
apt-get update
apt-get upgrade
apt-get dist-upgrade

# Download P5, and showing SHA1 to user, asking if he wants to proceed.
wget https://github.com/polterguy/phosphorusfive/releases/download/v5.7/binaries.zip
sha1sum binaries.zip

# Then asking user to confirm installation.
read -p "SHA1 of downloaded P5 zip file can be found above, continue? [y/n] " yn
if [[ ! $yn =~ ^[Yy]$ ]]; then
  exit
fi

# Installing MySQL server.
# Notice, by default MySQL is setup without networking, hence unless user explicitly opens it
# up later, this should be perfectly safe.
debconf-set-selections <<< 'mysql-server mysql-server/root_password password SomeRandomPassword'
debconf-set-selections <<< 'mysql-server mysql-server/root_password_again password SomeRandomPassword'
apt-get --assume-yes install apache2 mysql-server libapache2-mod-mono unzip gnupg2

# Disabling mod_mono_auto to make sure we create an "advanced" configuration for mod_mono
a2dismod mod_mono_auto

# Removing any old files.
# Notice, we don't remove "/common" and "/users" here.
# This allows for a nice upgrading process (hopefully) without loosing old data in your system.
rm /var/www/html/index.html
rm /var/www/html/Default.aspx
rm /var/www/html/Global.asax
rm /var/www/html/README.md
rm /var/www/html/startup.hl
rm -r -f /var/www/html/bin
rm -r -f /var/www/html/desktop
rm -r -f /var/www/html/modules

# Creating a temporary folder to hold output.
mkdir p5

# Unzipping P5, in addition to moving it into main www/html folder.
unzip binaries.zip -d p5
cp -R p5/* /var/www/html

# Removing both zip file, and temp folder created during above process.
rm -f binaries.zip
rm -f -r p5

# Editing web.config file, making sure we get the password correctly.
sed -i 's/User Id=root;/User Id=root;password=SomeRandomPassword;/g' /var/www/html/web.config

# Making GnuPG folder for Apache process.
    

# Giving ownership (recursively) to Apache user for entire folder.
# Necessary since P5 will create and modify its own file structure.
chown -R www-data:www-data /var/www

# Configuring mod_mono app
echo "
<apps>
        <web-application>
                <name>P5</name>
                <vpath>/</vpath>
                <path>/var/www/html</path>
                <vhost>example.com</vhost>
        </web-application>
</apps>
" > /etc/mono-server4/p5.webapp

# Configuring apache to accept requests for ASP.NET, and route them into Mono
echo "


# Phosphorus Five configurations
# These parts was added as a part of the install.sh script while installing Phosphorus Five.

MonoAutoApplication disabled
AddHandler mono .aspx .axd .config
MonoApplications \"/:/var/www/html\"
DirectoryIndex Default.aspx

# Turning OFF all options, and making sure they're impossible to override using .htaccess files, to be absolutely certain.
<Location />
    Options None
    AllowOverride None
</Location>

# Making sure Mono handles all requests that doesn't have an extension, to make our URL rewriting logic work.
<Location ~ \"^[^\.]*$\">
    SetHandler mono
</Location>

# Making sure we never serve Hyperlambda files.
<Files ~ \"\.hl$\">
    Order allow,deny
    Deny from all
</Files>

# Making user's private documents just that.
<Location ~ \"/users/[^/]/documents/private/\">
    Order allow,deny
    Deny from all
</Location>

# Making all temp files for users inaccessible.
<Location ~ \"/users/[^/]/temp/\">
    Order allow,deny
    Deny from all
</Location>

# Making all common private files just that.
<Location ~ \"/common/documents/private/\">
    Order allow,deny
    Deny from all
</Location>

# Making all common temp files private
<Location ~ \"/common/temp/\">
    Order allow,deny
    Deny from all
</Location>


# Phosphorus Five, further tightening server, to make it more secure.

# Disabling SSL2.0 and 3.0
SSLProtocol -ALL +TLSv1

# Disabling all handshakes on lower cipher suites
SSLCipherSuite ALL:!aNULL:!ADH:!eNULL:!LOW:!EXP:RC4+RSA:+HIGH:+MEDIUM

# Turning OFF tracing
TraceEnable off

# Turning OFF server signature and token
ServerSignature Off
ServerTokens Prod

# Turning OFF ETags, to avoid information leaking.
FileETag None

# Making sure server never returns a cookie which is not secure and HTTP only.
Header edit Set-Cookie ^(.*)$ \$1;HttpOnly;Secure

# Turning OFF mod_security for WebResource.axd
<Location \"/WebResource.axd\">
    <IfModule security2_module>
        SecRuleEngine Off
    </IfModule>
</Location>

# Turning OFF mod_security for folders where we need more slack.
<Location ~ \"/[^/]+/settings\">
    <IfModule security2_module>
        SecRuleEngine Off
    </IfModule>
</Location>
<Location \"/hypereval\">
    <IfModule security2_module>
        SecRuleEngine Off
    </IfModule>
</Location>
" >> /etc/apache2/apache2.conf

# Enabling headers modules in apache
a2enmod headers

# Installing SSL keys
apt-get install software-properties-common
add-apt-repository ppa:certbot/certbot
apt-get update
apt-get install python-certbot-apache
sudo certbot --apache

# Restarting Apache
service apache2 restart

# Informing user that his MySQL password can be found in web.config
echo "Your MySQL password can be found in the file '/var/www/html/web.config'"
