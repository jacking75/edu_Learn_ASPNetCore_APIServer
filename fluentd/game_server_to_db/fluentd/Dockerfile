FROM fluent/fluentd:v1.16-debian-1

# Use root account to use apt
USER root
EXPOSE 24224

# below RUN includes plugin as examples elasticsearch is not required
# you may customize including plugins as you wish
RUN buildDeps="sudo make gcc g++ libc-dev" \
    && apt-get update \
    && apt-get install -y --no-install-recommends $buildDeps \
    && sudo gem install fluent-plugin-sql \
    && sudo apt-get -y install default-libmysqlclient-dev \
    && sudo gem install mysql2 --no-document \
    && sudo gem install fluent-plugin-rewrite-tag-filter \
    && sudo gem sources --clear-all \
    && SUDO_FORCE_REMOVE=yes \
        apt-get purge -y --auto-remove \
                    -o APT::AutoRemove::RecommendsImportant=false \
                    $buildDeps \
    && rm -rf /var/lib/apt/lists/* \
    && rm -rf /tmp/* /var/tmp/* /usr/lib/ruby/gems/*/cache/*.gem

COPY fluent.conf /fluentd/etc/

USER fluent