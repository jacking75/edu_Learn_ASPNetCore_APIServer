FROM fluent/fluentd:edge-debian

USER root

# Avoid prompts
ENV DEBIAN_FRONTEND=noninteractive

# Install required packages for fluent-plugin-mysql
RUN apt-get update -y && \
    apt-get install -y \
    ubuntu-dev-tools \
    gcc \
    default-libmysqlclient-dev && \
    gem install fluent-plugin-mysql && \
    fluent-gem install fluent-plugin-rewrite-tag-filter && \
    gem sources --clear-all && \
    rm -rf /var/lib/apt/lists/* && \
    rm -rf /tmp/* /var/tmp/* /usr/lib/ruby/gems/*/cache/*.gem


COPY ./fluent.conf /fluentd/etc/fluent.conf
COPY ./db.conf /fluentd/etc/db.conf

EXPOSE 24224
CMD ["fluentd", "-c", "/fluentd/etc/fluent.conf"]