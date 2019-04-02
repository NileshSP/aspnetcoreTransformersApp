import React, { Component } from 'react';

export class WebApi extends Component {
    render() {
        return (
            <div className="embed-responsive embed-responsive-4by3">
                <iframe className="embed-responsive-item" src="/Swagger" title="Swagger UI" />
            </div>
        );
    }
}
