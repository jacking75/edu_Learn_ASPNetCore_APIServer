FROM fluent/fluentd:v1.17.1-debian-1.0

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
    gem sources --clear-all && \
    rm -rf /var/lib/apt/lists/* && \
    rm -rf /tmp/* /var/tmp/* /usr/lib/ruby/gems/*/cache/*.gem

USER fluent

COPY ./fluent.conf /fluentd/etc/fluent.conf

EXPOSE 24224
CMD ["fluentd", "-c", "/fluentd/etc/fluent.conf"]