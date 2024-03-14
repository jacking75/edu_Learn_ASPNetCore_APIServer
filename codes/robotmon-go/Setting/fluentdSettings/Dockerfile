FROM fluent/fluentd:v1.11-debian-1

# Use root account to use apt
USER root

# below RUN includes plugin as examples elasticsearch is not required

# you may customize including plugins as you wish

RUN buildDeps="sudo make gcc g++ libc-dev" \
 && apt-get update \
 && apt-get install -y --no-install-recommends $buildDeps \
 && sudo apt-get install -y libmariadb-dev \
 && sudo gem install fluent-plugin-elasticsearch \
 && sudo gem install fluent-plugin-out-http -v 0.1.4 \
 && sudo gem install fluent-plugin-mysql-bulk \
 && sudo gem sources --clear-all \
 && SUDO_FORCE_REMOVE=yes \
    apt-get purge -y --auto-remove \
                  -o APT::AutoRemove::RecommendsImportant=false \
                  $buildDeps \
 && rm -rf /var/lib/apt/lists/* \
 && rm -rf /tmp/* /var/tmp/* /usr/lib/ruby/gems/*/cache/*.gem

USER fluent