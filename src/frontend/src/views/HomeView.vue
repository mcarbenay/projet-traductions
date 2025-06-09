<template>
  <div class="sion-login-bg">
    <form class="sion-login" autocomplete="off">
      <div class="login-header">
        <div class="login-logo">
          <img src="/favicon.ico" alt="UseTheOps logo">
        </div>
        <div class="login-title-block">
          <div class="login-title">Polyglot Initiative</div>
          <div class="login-desc">Plateforme de gestion des traductions pour applications web &amp; mobile</div>
        </div>
      </div>
      <div class="statbar">
        <span>Uplink 97%</span>
        <span>CPU 41%</span>
        <span>Status: OK</span>
      </div>
      <div class="fake-gauges">
        <div class="gauge-block" data-percent="74">
          <div class="gauge-label">Flux données</div>
          <div class="gauge-bar-bg"><div class="gauge-bar-fill"></div></div>
        </div>
        <div class="gauge-block" data-percent="40">
          <div class="gauge-label">Trad. unit</div>
          <div class="gauge-bar-bg"><div class="gauge-bar-fill"></div></div>
        </div>
        <div class="gauge-block" data-percent="93">
          <div class="gauge-label">Res. mémoire</div>
          <div class="gauge-bar-bg"><div class="gauge-bar-fill"></div></div>
        </div>
      </div>
      <div class="form-row">
        <div>
          <label for="user">Identifiant</label>
          <input type="text" id="user" name="user" required autocomplete="off">
        </div>
        <div>
          <label for="pass">Mot de passe</label>
          <input type="password" id="pass" name="pass" required autocomplete="off">
        </div>
      </div>
      <button type="submit">Connexion</button>
      <div class="usedev-block">
        <div class="usedev-logo">
          <img src="/favicon.ico" alt="UseTheOps logo small">
        </div>
        <div class="usedev-name">UseTheOps</div>
      </div>
      <div class="footer">
        Système Polyglot Initiative // v1.0 © 2024
      </div>
    </form>
  </div>
</template>

<script setup lang="ts">
// Pas de logique pour la démo
</script>

<style scoped>
@import url('https://fonts.googleapis.com/css?family=Share+Tech+Mono&display=swap');
:root {
  --accent: #71c4ef;
  --graph: #b3cad7;
  --dark: #262931;
  --label: #aaa;
  --line: #e0e5ea;
}
.sion-login-bg {
  min-height: 100vh;
  background: #fcfcfd;
  display: flex;
  align-items: center;
  justify-content: center;
}
.sion-login {
  background: rgba(255,255,255,0.99);
  border: 1px solid var(--line);
  box-shadow: 0 8px 40px #bfd6ef33;
  border-radius: 12px;
  min-width: 390px;
  max-width: 96vw;
  padding: 2.4em 2.5em 1.9em 2.5em;
  display: flex;
  flex-direction: column;
  gap: 1.1em;
  position: relative;
  z-index: 1;
}
.sion-login::before, .sion-login::after {
  content: '';
  position: absolute;
  left: 0; right: 0;
  height: 2px;
  background: linear-gradient(to right, transparent, var(--line), transparent);
  opacity: 0.85;
  z-index: 0;
}
.sion-login::before {
  top: 3.7em;
}
.sion-login::after {
  bottom: 1.7em;
}
.login-header {
  display: flex;
  align-items: flex-start;
  gap: 1em;
  margin-bottom: .5em;
  margin-top: -.6em;
  position: relative;
  min-height: 56px;
}
.login-logo {
  width: 46px;
  min-width: 36px;
  height: 46px;
  background: #efefef;
  border-radius: 8px;
  border: 1px solid var(--line);
  box-shadow: 0px 2px 8px #71c4ef18;
  display: flex;
  align-items: center;
  justify-content: center;
  overflow: hidden;
  margin-right: 0.6em;
  margin-top: .05em;
}
.login-logo img {
  width: 33px;
  height: 33px;
  object-fit: cover;
  display: block;
}
.login-title-block {
  display: flex;
  flex-direction: column;
  gap: .13em;
}
.login-title {
  color: var(--accent);
  font-weight: bold;
  font-size: 1.13em;
  font-family: 'Share Tech Mono', monospace;
  letter-spacing: 0.12em;
  text-transform: uppercase;
  margin-bottom: .17em;
}
.login-desc {
  color: var(--label);
  font-size: .92em;
  letter-spacing: .04em;
  margin-left: .02em;
  opacity: .93;
  margin-bottom: 0;
}
.statbar {
  display: flex;
  justify-content: flex-end;
  gap: 1.2em;
  font-size: 0.95em;
  color: var(--label);
  border-bottom: 1px solid var(--line);
  padding-bottom: .35em;
  margin-bottom: -.2em;
  opacity: 0.89;
}
.fake-gauges {
  display: flex;
  align-items: flex-end;
  gap: 2.1em;
  opacity: .77;
}
.gauge-block {
  flex: 1 0 0;
  font-size: .91em;
  color: var(--graph);
  display: flex;
  flex-direction: column;
  gap: .19em;
}
.gauge-label {
  margin-bottom: 0.13em;
  color: var(--label);
  font-size: .79em;
}
.gauge-bar-bg {
  background: var(--line);
  border-radius: 1px;
  height: 5px;
  margin-bottom: 3px;
  position: relative;
  overflow: hidden;
}
.gauge-bar-fill {
  height: 100%;
  border-radius: 1px;
  background: var(--accent);
  opacity: .55;
  width: 50%;
  transition: width .6s cubic-bezier(.25,.78,.18,.96);
}
.gauge-block[data-percent="74"] .gauge-bar-fill { width: 74%; }
.gauge-block[data-percent="40"] .gauge-bar-fill { width: 40%; }
.gauge-block[data-percent="93"] .gauge-bar-fill { width: 93%; }
.form-row {
  margin-top: .7em;
  margin-bottom: .40em;
  display: flex;
  flex-direction: column;
  gap: 0.7em;
}
label {
  color: var(--label);
  font-size: .92em;
  margin-bottom: .13em;
  letter-spacing: .025em;
  opacity: .97;
}
input {
  background: none;
  border: none;
  border-bottom: 1.3px solid var(--line);
  color: var(--dark);
  font-family: 'Share Tech Mono', monospace;
  font-size: 1.04em;
  padding: 0.32em 0em .20em .1em;
  transition: border-color 0.19s;
}
input:focus {
  border-color: var(--accent);
  outline: none;
  background: #f3f8fa;
}
button {
  margin-top: 1.2em;
  letter-spacing: 0.12em;
  border: 1.5px solid var(--accent);
  color: var(--accent);
  padding: 0.62em 0;
  background: none;
  width: 100%;
  text-transform: uppercase;
  font-size: 1.05em;
  font-family: 'Share Tech Mono', monospace;
  border-radius: 4px;
  font-weight: bold;
  cursor: pointer;
  transition: background .14s, color .14s;
}
button:hover, button:focus {
  color: #fff;
  background: var(--accent);
  border-color: var(--accent);
}
.footer {
  font-size: .93em;
  color: var(--label);
  text-align: center;
  letter-spacing: .05em;
  opacity: .65;
  margin-top: 2.7em;
}
.usedev-block {
  display: flex;
  justify-content: flex-end;
  align-items: center;
  gap: .3em;
  margin-top: -2em;
  margin-bottom: 2.1em;
  margin-right: -.9em;
}
.usedev-logo {
  height: 18px;
  width: 18px;
  flex-shrink: 0;
  border-radius: 4px;
  background: #fff;
  overflow: hidden;
  border: 1px solid #dde2e7;
}
.usedev-logo img {
  width: 100%;
  height: 100%;
  object-fit: cover;
  display: block;
}
.usedev-name {
  font-size: .92em;
  color: #b3cad7;
  font-weight: normal;
  letter-spacing: .07em;
  text-transform: uppercase;
  opacity: .84;
}
@media (max-width:600px){
  .sion-login {min-width:0; padding: 1em;}
  .login-header {flex-direction:column; gap:.7em;}
  .login-logo {margin-bottom: .4em;}
  .usedev-block{margin-top:.4em;}
  .footer{font-size:.87em; margin-top: 1.2em; text-align: center;}
}
</style>
