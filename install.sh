#!/bin/bash

clear

#                         *********************************************
#                         *     About this script                     *
#                         *     This script will do the following     *
#                         *********************************************
#
#   1. Upgrade your Linux distro, and install all updates
#   2. Install "uncomplicated firewall", and shut down all ports except 22 (SSH), 80 (HTTP) and 443 (HTTPS)
#   3. Install Apache
#   4. Install MySQL, without network drivers, and the password of "ThisIsNotANicePassword" (consider changing this if you like).
#      Although this is technically not important, since no network drivers are enabled in MySQL anyways, and all networks
#      ports are anyways closed by "ufw".
#   5. Install Mono
#   6. Install mod_mono (Apache bindings), and disable the "auto configuration" module
#   7. Install unzip
#   8. Give ownership of your entire /var/www folder, recursively to your Apache user
#   9. Configure your /var/www/html folder to accept ASP.NET requests, and route these to mod_mono (Mono's ASP.NET process)
#  10. Create the file "/etc/apache2/phosphorus.conf", which contains general Phosphorus Settings, in addition to
#      some Mono settings, and include this file into your main Apache configuration file, which allows you to run your
#      website as an ASP.NET/Mono website.
#  11. Then it will ask you if you want to install an SSL certificate from "Let's Encrypt", at which point if you
#      answer yes to this, you must already have a domain setup, and a DNS record pointing to your server's IP address.
#      The script will create a cron job, automatically renewing your SSL keypair if you choose to instal an SSL keypair.
#  12. Download the binary release of Phosphorus Five, unzip it, and copy all files into your main Apache folder.
#
#      Notice, this script is created explicitly to install a Phosphorus Five server, but it can probably be
#      modified to create a highly secure "generic ASP.NET/Mono" WebSite for Apache if you modify it slightly.
#      The script has only been tested with Ubuntu servers, however it _might_ also work with other Debian
#      based systems.
#      This script will _significantly_ increase the security of your server.
#      The script is intended to be executed on a "vanilla" Ubuntu Linux server, implying a newly setup
#      "clean" Ubuntu server.
#
#      WARNING - It will delete all files and folders you have from before in your main Apache folder.
#      Consider creating a backup of these files, if you are upgrading an existing server!
#
#      If you have an existing MySQL server running, or MySQL locally installed on your server from before,
#      you might have to manually edit the /var/www/html/web.config file, and change the MySQL connection string
#      after having executed this script.
#      In general terms the script is "greedy", and requires your entire Apache folder for its own things.
#      But you can probably manually edit this afterwards, if you'd like for your server to have specific folder(s),
#      from where it runs (for instance) PHP website(s), etc.
#
#      The script follows most "best practices" in regards to tightening your web server, such as turning
#      off server identification, versioning, ETags, etc.
#      In addition, it correctly tightens the security of all your special Phosphorus Five folders, such as
#      your users' private folders, to prevent direct downloading of files, etc.


# First giving user some information about what this script actually does.

echo "================================================================================"
echo "Automatic installation script for Phosphorus Five."
echo "Please let it finish, without interruptions, which might take some time."
echo "Notice, Phosphorus Five is licensed as GPLv3."
echo ""
echo "This script will also update, upgrade, and further secure your Linux server."
echo ""
echo "This script will periodically require input from you."
echo ""
echo "The software is distributed in the hope that it will be useful,"
echo "but WITHOUT ANY WARRANTY; without even the implied warranty of"
echo "MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the"
echo "GNU General Public License for more details."
echo "================================================================================"
echo ""

# Asking user if he wants to continue.
read -p "Do you wish to proceed? [y/n] " yn
if [[ ! $yn =~ ^[Yy]$ ]]; then
  exit
fi

# Making sure we're able to install latest version of Mono
# Ubuntu's version is hopelessly outdated!!
sudo apt-key adv --keyserver hkp://keyserver.ubuntu.com:80 --recv-keys 3FA7E0328081BFF6A14DA29AA6A19B38D3D831EF
echo "deb http://download.mono-project.com/repo/ubuntu stable-xenial main" | sudo tee /etc/apt/sources.list.d/mono-official-stable.list


# Making sure we update and upgrade our server.
sudo apt-get update
sudo apt-get upgrade
sudo apt-get dist-upgrade

# Downloading the latest release.
wget https://github.com/polterguy/phosphorusfive/releases/download/v8.4/binaries.zip

# Installing MySQL server.
# Notice, by default MySQL is setup without networking, hence unless user explicitly opens it.
# up later, this should be perfectly safe.
sudo debconf-set-selections <<< 'mysql-server mysql-server/root_password password ThisIsNotANicePassword'
sudo debconf-set-selections <<< 'mysql-server mysql-server/root_password_again password ThisIsNotANicePassword'
sudo apt-get --assume-yes install apache2 mysql-server libapache2-mod-mono unzip ufw

# Disabling mod_mono_auto to make sure we create an "advanced" configuration for mod_mono.
sudo a2dismod mod_mono_auto

# Turning on firewall for everything except SSH, HTTP and HTTPS
sudo ufw allow 80
sudo ufw allow 443
sudo ufw allow 22
sudo ufw enable

# Removing any old files.
# Notice, we don't remove "/common" and "/users" here.
# This allows for a nice upgrading process (hopefully) without loosing old data in your system.
sudo rm /var/www/html/index.html
sudo rm /var/www/html/Default.aspx
sudo rm /var/www/html/Global.asax
sudo rm /var/www/html/README.md
sudo rm /var/www/html/startup.hl
sudo rm -r -f /var/www/html/bin
sudo rm -r -f /var/www/html/modules

# Creating a temporary folder to hold output.
mkdir p5

# Unzipping P5, in addition to moving it into main www/html folder, and making sure
# the Apache user has full control over folders.
unzip binaries.zip -d p5
sudo cp -R p5/* /var/www/html

# Removing both zip file, and temp folder created during above process.
rm -f binaries.zip
rm -f -r p5

# Making GnuPG folder for Apache process.
sudo mkdir /var/www/.gnupg

# Giving ownership (recursively) to Apache user for entire folder.
# Necessary since P5 will create and modify its own file structure.
sudo chown -R www-data:www-data /var/www

# Configuring mod_mono app
sudo echo "
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
sudo echo "

#############################################################
#
# Phosphorus Five configuration
#
#############################################################

MonoAutoApplication disabled
AddHandler mono .aspx .axd .config .asax .asmx
MonoApplications \"/:/var/www/html\"
DirectoryIndex Default.aspx

# Turning OFF all options, and making sure they're impossible to override using .htaccess files, to be absolutely certain.
# These are simple security measures that further tights down our web app.
<Location />
    Options None
    AllowOverride None
</Location>

# Making sure Mono handles all requests that doesn't contain a dot (.), to make our URL rewriting logic work.
<Location ~ \"^[^\.]*$\">
    SetHandler mono
</Location>

# Making sure we never serve Hyperlambda files.
<Files ~ \"\.hl$\">
    Order allow,deny
    Deny from all
</Files>

# Notice, these next steps will make sure our users and common folders have
# the right type of protection for direct file access.
# Basically, anything in 'public' whatever is available through a direct link,
# everything else will never be served

# Making user's private documents just that.
<Location ~ \"/users/[^/]+/documents/private/\">
    Order allow,deny
    Deny from all
</Location>

# Making all temp files for users inaccessible.
<Location ~ \"/users/[^/]+/temp/\">
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

# Turning OFF tracing
TraceEnable off

# Turning OFF server signature and token
ServerSignature Off
ServerTokens Prod

# Turning OFF ETags, to avoid information leaking.
FileETag None
" > /etc/apache2/phosphorusfive.conf



# Modifying apache2.conf file, but only if it is necessary
if ! grep -q 'Include /etc/apache2/phosphorusfive.conf' /etc/apache2/apache2.conf; then
  sudo echo "

#############################################################
#
# Includes Phosphorus Five configuration file
#
#############################################################

Include /etc/apache2/phosphorusfive.conf

" >> /etc/apache2/apache2.conf
fi






# Installing SSL keys, if user wants to.
echo "Do you wish to install an SSL keypair on your server?"
echo "This step requires a pre-configured domain and DNS record."
read -p "[y/n] " yn
if [[ $yn =~ ^[Yy]$ ]]; then

  # Standard installation instructions, according to Let's Encrypt
  sudo apt-get install software-properties-common
  sudo add-apt-repository ppa:certbot/certbot
  sudo apt-get update
  sudo apt-get install python-certbot-apache
  sudo certbot --apache

  # Creating a CRON job, auto-renewing SSL keys if it's time.
  sudo echo "
  export HOME=\"/root\"
  export PATH=\"\${PATH}:/usr/local/sbin:/usr/local/bin:/usr/sbin:/usr/bin:/sbin:/bin\"
  certbot-auto --no-self-upgrade certonly
" > /etc/cron.daily/certbot-renew
  chmod a+x /etc/cron.daily/certbot-renew
fi

# Restarting Apache.
sudo service apache2 restart

# Informing user that his MySQL password can be found in web.config.
echo "Your MySQL password can be found in the file '/var/www/html/web.config'"
